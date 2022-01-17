using DNTYD.Core.Entities;

namespace DNTYD.Core.Contracts.Responses.Tracking; 

public class GetTrackingPointsRequest : BaseResponse {
	public IEnumerable<TrackingPoint> TrackingPoints { get; set; } = Enumerable.Empty<TrackingPoint>();
}