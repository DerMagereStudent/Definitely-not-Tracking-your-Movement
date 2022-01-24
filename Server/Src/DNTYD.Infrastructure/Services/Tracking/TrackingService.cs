using DNTYD.Core.Contracts.Requests.Tracking;
using DNTYD.Core.Contracts.Responses.Tracking;
using DNTYD.Core.Entities;
using DNTYD.Core.Enums;
using DNTYD.Core.Services.Identity;
using DNTYD.Core.Services.Tracking;
using DNTYD.Infrastructure.Database;
using DNTYD.Infrastructure.Database.Models;

using Microsoft.AspNetCore.Identity;

namespace DNTYD.Infrastructure.Services.Tracking; 

public class TrackingService : ITrackingService<string> {
	private readonly ApplicationDbContext _dbContext;
	private readonly ILoginService<IdentityUser> _loginService;

	public TrackingService(ApplicationDbContext dbContext, ILoginService<IdentityUser> loginService) {
		this._dbContext = dbContext;
		this._loginService = loginService;
	}
	
	public async Task<AddTrackingPointResponse> AddTrackingPointAsync(AddTrackingPointRequest<string> request, string token) {
		IdentityUser? user = await this._loginService.GetUserForTokenAsync(token);

		if (user is null) {
			return new AddTrackingPointResponse {
				Errors = new[] {AddTrackingPointResponse.Error.InvalidToken}
			};
		}
		
		string targetUserId = string.IsNullOrWhiteSpace(request.UserId) ? user.Id : request.UserId;

		// if user is not setting own points and is not admin (so trying to set someone else data without admin rights)
		if (!targetUserId.Equals(user.Id) && !await this._loginService.UserHasRoleAsync(user, UserRoles.Admin.ToString())) {
			return new AddTrackingPointResponse {
				Errors = new[] {AddTrackingPointResponse.Error.Unauthorized}
			};
		}
		
		TrackingPointModel trackingPoint = new TrackingPointModel {
			UserId = user.Id,
			Latitude = request.TrackingPoint.Latitude,
			Longitude = request.TrackingPoint.Longitude,
			TimeStampTracked = request.TrackingPoint.TimeStampTracked?.ToUniversalTime() ?? DateTime.UtcNow
		};

		this._dbContext.TrackingPoints.Add(trackingPoint);
		await this._dbContext.SaveChangesAsync();

		return new AddTrackingPointResponse {
			Succeeded = true,
			Messages = new[] {AddTrackingPointResponse.Message.TrackingPointAdded}
		};
	}

	public async Task<GetTrackingPointsResponse> GetTrackingPointsAsync(GetTrackingPointsRequest<string> request, string token) {
		IdentityUser? user = await this._loginService.GetUserForTokenAsync(token);

		if (user is null) {
			return new GetTrackingPointsResponse {
				Errors = new[] {GetTrackingPointsResponse.Error.InvalidToken}
			};
		}

		string targetUserId = string.IsNullOrWhiteSpace(request.UserId) ? user.Id : request.UserId;

		// if user is not getting own points and is not admin (so trying to get someone else data without admin rights)
		if (!targetUserId.Equals(user.Id) && !await this._loginService.UserHasRoleAsync(user, UserRoles.Admin.ToString())) {
			return new GetTrackingPointsResponse {
				Errors = new[] {GetTrackingPointsResponse.Error.Unauthorized}
			};
		}

		DateTime minTimeStamp = request.MinTimeStampTracked ?? DateTime.MinValue;
		
		return new GetTrackingPointsResponse {
			Succeeded = true,
			TrackingPoints = this._dbContext.TrackingPoints.Where(tp => tp.UserId.Equals(user.Id) && tp.TimeStampTracked > minTimeStamp),
			Messages = new[] {GetTrackingPointsResponse.Message.TrackingPointsReturned}
		};
	}
}