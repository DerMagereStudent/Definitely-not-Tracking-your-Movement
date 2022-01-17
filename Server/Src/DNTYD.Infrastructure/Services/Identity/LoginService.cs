using System.Security.Claims;

using DNTYD.Core.Contracts.Requests.Identity;
using DNTYD.Core.Contracts.Responses.Identity;
using DNTYD.Core.Options;
using DNTYD.Core.Services;
using DNTYD.Core.Services.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DNTYD.Infrastructure.Services.Identity; 

public class LoginService : ILoginService<IdentityUser> {
	private readonly UserManager<IdentityUser> _userManager;
	private readonly IJwtIssuingService _jwtIssuingService;
	private readonly JwtIssuingOptions _jwtIssuingOptions;

	public LoginService(UserManager<IdentityUser> userManager, IJwtIssuingService jwtIssuingService, IOptions<JwtIssuingOptions> jwtIssuingOptions) {
		this._userManager = userManager;
		this._jwtIssuingService = jwtIssuingService;
		this._jwtIssuingOptions = jwtIssuingOptions.Value;
	}

	public async Task<LoginResponse> LoginUserAsync(LoginRequest request) {
		IdentityUser user = await this._userManager.FindByEmailAsync(request.UsernameEmail);
			
		if (user is null) {
			user = await this._userManager.FindByNameAsync(request.UsernameEmail);

			if (user is null) {
				return new LoginResponse {
					Errors = new[] {LoginResponse.Error.UsernameOrEmailNotRegistered}
				};
			}
		}
			
		bool validPassword = await this._userManager.CheckPasswordAsync(user, request.Password);

		if (!validPassword) {
			return new LoginResponse {
				Errors = new[] {LoginResponse.Error.PasswordNotCorrect}
			};
		}

		string token = this._jwtIssuingService.IssueToken(
			this._jwtIssuingOptions.Secret, user.Id, user.Email, this._jwtIssuingOptions.ExpiringTime,
			(await this._userManager.GetRolesAsync(user)).Select(
				r => new Claim(ClaimTypes.Role, r)
			).ToArray()
		);

		return new LoginResponse {
			Succeeded = true,
			Token = token,
			Messages = new[] {LoginResponse.Message.LoggedIn}
		};
	}

	public async Task<IdentityUser?> GetUserForTokenAsync(string token) {
		string? userId = this._jwtIssuingService.GetSubFromToken(token);

		if (userId is null)
			return null;

		return await this._userManager.FindByIdAsync(userId);
	}

	public async Task<bool> UserHasRoleAsync(IdentityUser user, string role) {
		return await this._userManager.IsInRoleAsync(user, role);
	}
}