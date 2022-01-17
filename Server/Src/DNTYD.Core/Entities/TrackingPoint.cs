using System.ComponentModel.DataAnnotations;

namespace DNTYD.Core.Entities; 

public class TrackingPoint {
	[Required, Range(-90f, 90f, ErrorMessage = "The valid range of latitude in degrees is -90 and +90 for the southern and northern hemispheres, respectively.")]
	public float Latitude { get; set; }
	
	[Required, Range(-180f, 180f, ErrorMessage = "The valid range of longitude in degrees is -180 and +180 specifying coordinates west and east of the Prime Meridian, respectively.")]
	public float Longitude { get; set; }
	
	public DateTime? TimeStampTracked { get; set; }
	
	public string? Address { get; set; }
}