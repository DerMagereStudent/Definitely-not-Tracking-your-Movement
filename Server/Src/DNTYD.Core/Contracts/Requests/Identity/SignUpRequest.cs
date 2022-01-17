using System.ComponentModel.DataAnnotations;

namespace DNTYD.Core.Contracts.Requests.Identity;

public class SignUpRequest {
	[Required] public string Username { get; set; }
	[Required] public string Email { get; set; }
	[Required] public string Password { get; set; }
}