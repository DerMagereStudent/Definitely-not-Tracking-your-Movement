using DNTYD.Core.Entities;

namespace DNTYD.Core.Contracts.Responses.Tracking; 

public class GetTrackingPointsResponse : BaseResponse {
	public IEnumerable<TrackingPoint> TrackingPoints { get; set; } = Enumerable.Empty<TrackingPoint>();
	
	public static class Message {
		public static readonly DNTYD.Core.ValueObjects.Message TrackingPointsReturned = new() {
			Code = nameof(TrackingPointsReturned),
			Description = "The tracking points for the requested user were returned"
		};
	}
	
	public static class Error {
		public static readonly DNTYD.Core.ValueObjects.Message InvalidToken = new() {
			Code = nameof(InvalidToken),
			Description = "The token is invalid due to wrong or missing information"
		};
		
		public static readonly DNTYD.Core.ValueObjects.Message Unauthorized = new() {
			Code = nameof(Unauthorized),
			Description = "The user is unauthorized the get this data"
		};
		
		public static readonly DNTYD.Core.ValueObjects.Message InternalError = new () {
			Code = nameof(InternalError),
			Description = "Putting the gps tracking point failed due to an internal server error"
		};
	}
}