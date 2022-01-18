using DNTYD.Core.Contracts.Requests.Identity;
using DNTYD.Core.Contracts.Responses.Identity;
using DNTYD.Core.Services.Identity;
using DNTYD.Core.ValueObjects;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DNTYD.WebAPI.Controllers.Identity; 

[ApiController]
[Route("api/identity")]
public class IdentityController : ControllerBase {
	private readonly ISignUpService _signUpService;
	private readonly ILoginService<IdentityUser> _loginService;
	private readonly ILogger<IdentityController> _logger;
	
	public IdentityController(ISignUpService signUpService, ILoginService<IdentityUser> loginService, ILogger<IdentityController> logger) {
		this._signUpService = signUpService;
		this._loginService = loginService;
		this._logger = logger;
	}

	[HttpPost]
	[Route("signup")]
	public async Task<IActionResult> SignUpUserAsync([FromBody] SignUpRequest request) {
		if (!this.ModelState.IsValid) {
			return this.BadRequest(new SignUpResponse {
				Errors = this.ModelState.Values.SelectMany(v => v.Errors).Select(e => new Message {
					Code = e.Exception is null ? "" : e.Exception.GetType().Name,
					Description = e.ErrorMessage
				})
			});
		}

		try {
			return this.Ok(await this._signUpService.SignUpUserAsync(request));
		}
		catch (Exception e) {
			this._logger.LogError(e, $"{nameof(this.SignUpUserAsync)} threw an exception");
			return this.StatusCode(500, new SignUpResponse {
				Errors = new[] {
					new Message {
						Code = e.GetType().Name,
						Description = e.Message
					}
				}
			});
		}
	}

	[HttpPost]
	[Route("login")]
	public async Task<IActionResult> LoginUserAsync([FromBody] LoginRequest request) {
		if (!this.ModelState.IsValid) {
			return this.BadRequest(new LoginResponse {
				Errors = this.ModelState.Values.SelectMany(v => v.Errors).Select(e => new Message {
					Code = e.Exception is null ? "" : e.Exception.GetType().Name,
					Description = e.ErrorMessage
				})
			});
		}
		
		try {
			return this.Ok(await this._loginService.LoginUserAsync(request));
		}
		catch (Exception e) {
			this._logger.LogError(e, $"{nameof(this.LoginUserAsync)} threw an exception");
			return this.StatusCode(500, new LoginResponse {
				Errors = new[] {
					new Message {
						Code = e.GetType().Name,
						Description = e.Message
					}
				}
			});
		}
	}
}