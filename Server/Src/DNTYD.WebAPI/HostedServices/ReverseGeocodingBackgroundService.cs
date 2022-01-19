using System.Net;

using DNTYD.Core.Entities;
using DNTYD.Infrastructure.Database;
using DNTYD.Infrastructure.Database.Models;

using Microsoft.EntityFrameworkCore;

namespace DNTYD.WebAPI.HostedServices; 

public class ReverseGeocodingBackgroundService : BackgroundService {
	private const int NominatimApiRequestBreak = 1000;
	
	private readonly IServiceScopeFactory  _serviceScopeFactory;
	private readonly ILogger<ReverseGeocodingBackgroundService> _logger;
	
	private ApplicationDbContext _applicationDbContext = null!;
	
	public ReverseGeocodingBackgroundService(IServiceScopeFactory  serviceScopeFactory, ILogger<ReverseGeocodingBackgroundService> logger) {
		this._serviceScopeFactory = serviceScopeFactory;
		this._logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		using IServiceScope scope = this._serviceScopeFactory.CreateScope();
		this._applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		
		await this.RunAddressResolvingLoop(stoppingToken);
	}

	private async Task RunAddressResolvingLoop(CancellationToken stoppingToken) {
		while (!stoppingToken.IsCancellationRequested) {
			await Task.Delay(ReverseGeocodingBackgroundService.NominatimApiRequestBreak, stoppingToken);
			
			TrackingPointModel? point = await this.GetNextPendingTrackingPoint(stoppingToken);

			if (point is null)
				continue;

			string? address = await this.GetAddressForTrackingPoint(point, stoppingToken);
			
			if (string.IsNullOrWhiteSpace(address))
				continue;

			point.Address = address;
			await this.SaveTrackingPoint(point, stoppingToken);
		}
	}

	private async Task<TrackingPointModel?> GetNextPendingTrackingPoint(CancellationToken stoppingToken) {
		if (stoppingToken.IsCancellationRequested)
			return null;

		try {
			return await this._applicationDbContext.TrackingPoints.FirstOrDefaultAsync(tp => string.IsNullOrWhiteSpace(tp.Address), stoppingToken);
		}
		catch (Exception e) {
			this._logger.LogError(e, $"{nameof(this.GetNextPendingTrackingPoint)} threw an exception");
			return null;
		}
	}

	private async Task<string?> GetAddressForTrackingPoint(TrackingPoint point, CancellationToken stoppingToken) {
		try {
			HttpWebRequest request = WebRequest.CreateHttp($"https://nominatim.openstreetmap.org/reverse?lat={point.Latitude}&lon={point.Longitude}&format=geojson");
			request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36";
		
			HttpWebResponse? response = await request.GetResponseAsync() as HttpWebResponse;

			if (response is null || response.StatusCode != HttpStatusCode.OK)
				return null;
			
			return await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
			// just return the response data containing all the address information in a geojson string.
			// Is not worth parsing because its only used in the frontend and js can parse is automatically.  
		}
		catch (Exception e) {
			this._logger.LogError(e, $"{nameof(this.GetAddressForTrackingPoint)} threw an exception");
			return null;
		}
	}

	private async Task SaveTrackingPoint(TrackingPointModel point, CancellationToken stoppingToken) {
		try {
			await this._applicationDbContext.SaveChangesAsync(stoppingToken);
		}
		catch (Exception e) {
			this._logger.LogError(e, $"{nameof(this.SaveTrackingPoint)} threw an exception");
		}
	}
}