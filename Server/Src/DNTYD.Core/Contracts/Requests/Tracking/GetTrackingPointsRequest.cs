namespace DNTYD.Core.Contracts.Requests.Tracking; 

public class GetTrackingPointsRequest<TUserId> {
	public TUserId? UserId { get; set; }
	public DateTime? MinTimeStampTracked { get; set; }
}