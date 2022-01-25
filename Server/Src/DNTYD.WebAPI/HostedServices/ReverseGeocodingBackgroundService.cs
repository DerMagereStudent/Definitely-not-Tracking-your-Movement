using System.Diagnostics;
using System.Globalization;
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
		Stopwatch stopwatch = new Stopwatch();
		while (!stoppingToken.IsCancellationRequested) {
			stopwatch.Stop();
			await Task.Delay((int)Math.Max(0, ReverseGeocodingBackgroundService.NominatimApiRequestBreak - stopwatch.ElapsedMilliseconds), stoppingToken);
			stopwatch.Restart();
			
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
			/*this._logger.LogInformation($"Latitude:  {point.Latitude.ToString(CultureInfo.InvariantCulture)}");
			this._logger.LogInformation($"Longitude: {point.Longitude.ToString(CultureInfo.InvariantCulture)}");*/
			
			HttpRequestMessage message = new HttpRequestMessage();
			message.Method = HttpMethod.Get;
			message.RequestUri = new Uri($"https://nominatim.openstreetmap.org/reverse?lat={point.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={point.Longitude.ToString(CultureInfo.InvariantCulture)}&format=geojson");

			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36");
			HttpResponseMessage response = await client.SendAsync(message, stoppingToken);

			if (response.StatusCode != HttpStatusCode.OK) {
				this._logger.LogError(await response.Content.ReadAsStringAsync(stoppingToken));
				return null;
			}

			return await response.Content.ReadAsStringAsync(stoppingToken);

			/*HttpWebRequest request = WebRequest.CreateHttp($"https://nominatim.openstreetmap.org/reverse?lat={point.Latitude}&lon={point.Longitude}&format=geojson");
			request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.71 Safari/537.36";
		
			HttpWebResponse? response = await request.GetResponseAsync() as HttpWebResponse;

			if (response is null || response.StatusCode != HttpStatusCode.OK)
				return null;
			
			return await new StreamReader(response.GetResponseStream()).ReadToEndAsync();*/
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