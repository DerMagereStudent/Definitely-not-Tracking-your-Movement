namespace DNTYD.Core.Options; 

public class JwtIssuingOptions {
	public const string Key = nameof(JwtIssuingOptions);
		
	public string Secret { get; set; }
	public TimeSpan ExpiringTime { get; set; }
}