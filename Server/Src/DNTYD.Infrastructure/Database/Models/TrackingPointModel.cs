using DNTYD.Core.Entities;

using Microsoft.AspNetCore.Identity;

namespace DNTYD.Infrastructure.Database.Models; 

public class TrackingPointModel : TrackingPoint {
	public string UserId { get; set; }
	public IdentityUser User { get; set; }
}