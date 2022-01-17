using DNTYD.Core.Contracts.Requests.Identity;
using DNTYD.Core.Contracts.Responses.Identity;

namespace DNTYD.Core.Services.Identity; 

public interface ILoginService<TUser> {
	Task<LoginResponse> LoginUserAsync(LoginRequest request);
	Task<TUser?> GetUserForTokenAsync(string token);
	Task<bool> UserHasRoleAsync(TUser user, string role);
}