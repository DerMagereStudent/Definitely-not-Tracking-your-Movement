using System.ComponentModel.DataAnnotations;

using DNTYD.Core.Entities;

namespace DNTYD.Core.Contracts.Requests.Tracking; 

public class AddTrackingPointRequest<TUserId> {
	public TUserId? UserId { get; set; }
	
	[Required]
	public TrackingPoint TrackingPoint { get; set; }
}