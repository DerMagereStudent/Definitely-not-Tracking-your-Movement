using System.Transactions;

using DNTYD.Core.Contracts.Requests.Identity;
using DNTYD.Core.Contracts.Responses.Identity;
using DNTYD.Core.Enums;
using DNTYD.Core.Services.Identity;
using DNTYD.Core.ValueObjects;

using Microsoft.AspNetCore.Identity;

namespace DNTYD.Infrastructure.Services.Identity; 

public class SignUpService : ISignUpService {
	private readonly UserManager<IdentityUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
		
	public SignUpService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) {
		this._userManager = userManager;
		this._roleManager = roleManager;
	}
	
	public async Task<SignUpResponse> SignUpUserAsync(SignUpRequest request) {
		IdentityUser user = new IdentityUser() {
			UserName = request.Username,
			Email = request.Email
		};

		// Create a sql transaction to revoke if role creation fails, so no user can exist without a role.
		using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
			IdentityResult creationResult = await this._userManager.CreateAsync(user, request.Password);

			if (!creationResult.Succeeded) {
				return new SignUpResponse {
					Errors = creationResult.Errors.Select(error => new Message() {
						Code = error.Code,
						Description = error.Description
					})
				};
			}

			if (!await this._roleManager.RoleExistsAsync(UserRoles.Basic.ToString()))
				await this._roleManager.CreateAsync(new IdentityRole(UserRoles.Basic.ToString()));
					
			IdentityResult roleAssignmentResult = await this._userManager.AddToRoleAsync(user, UserRoles.Basic.ToString());

			if (!roleAssignmentResult.Succeeded) {
				return new SignUpResponse {
					Errors = roleAssignmentResult.Errors.Select(error => new Message() {
						Code = error.Code,
						Description = error.Description
					})
				};
			}

			scope.Complete();
		}

		return new SignUpResponse {
			Succeeded = true,
			Messages = new[] {SignUpResponse.Message.SignedUp}
		};
	}
}