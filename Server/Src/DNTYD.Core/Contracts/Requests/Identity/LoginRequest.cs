using System.ComponentModel.DataAnnotations;

namespace DNTYD.Core.Contracts.Requests.Identity;

public class LoginRequest {
	[Required] public string UsernameEmail { get; set; }
	[Required] public string Password { get; set; }
}