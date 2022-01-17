using DNTYD.Core.Contracts.Requests.Tracking;
using DNTYD.Core.Contracts.Responses.Tracking;

namespace DNTYD.Core.Services.Tracking; 

public interface ITrackingService<TUserId> {
	Task<AddTrackingPointResponse> AddTrackingPointAsync(AddTrackingPointRequest<TUserId> request, string token);
	Task<GetTrackingPointsResponse> GetTrackingPointsAsync(GetTrackingPointsRequest<TUserId> request, string token);
}