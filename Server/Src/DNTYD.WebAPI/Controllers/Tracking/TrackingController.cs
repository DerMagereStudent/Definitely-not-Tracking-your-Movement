using DNTYD.Core.Contracts.Requests.Tracking;
using DNTYD.Core.Contracts.Responses.Tracking;
using DNTYD.Core.Services.Tracking;
using DNTYD.Core.ValueObjects;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DNTYD.WebAPI.Controllers.Tracking; 

[ApiController]
[Route("api/tracking")]
public class TrackingController : ControllerBase {
	private readonly ITrackingService<string> _trackingService;
	private readonly ILogger<TrackingController> _logger;
	
	public TrackingController(ITrackingService<string> trackingService, ILogger<TrackingController> logger) {
		this._trackingService = trackingService;
		this._logger = logger;
	}

	[Authorize]
	[HttpPut]
	[Route("add-position")]
	public async Task<IActionResult> AddTrackingPointAsync([FromBody] AddTrackingPointRequest<string> request) {
		if (!this.ModelState.IsValid) {
			return this.BadRequest(new AddTrackingPointResponse {
				Errors = this.ModelState.Values.SelectMany(v => v.Errors).Select(e => new Message {
					Code = e.Exception is null ? "" : e.Exception.GetType().Name,
					Description = e.ErrorMessage
				})
			});
		}

		try {
			AddTrackingPointResponse response = await this._trackingService.AddTrackingPointAsync(
				request,
				this.Request.Headers[HeaderNames.Authorization]
			);
			
			if (!response.Succeeded && response.Errors.Any(e =>
				    e.Code.Equals(AddTrackingPointResponse.Error.InvalidToken.Code) ||
				    e.Code.Equals(AddTrackingPointResponse.Error.Unauthorized.Code))) {
				return this.Unauthorized(response);
			}
			
			return this.Ok(response);
		}
		catch (Exception e) {
			this._logger.LogError(e, $"{nameof(this.AddTrackingPointAsync)} threw an exception");
			return this.StatusCode(500, new AddTrackingPointResponse {
				Errors = new[] {
					new Message {
						Code = e.GetType().Name,
						Description = e.Message
					}
				}
			});
		}
	}

	[Authorize]
	[HttpPut]
	[Route("positions")]
	public async Task<IActionResult> GetTrackingPointsAsync([FromQuery] GetTrackingPointsRequest<string> request) {
		if (!this.ModelState.IsValid) {
			return this.BadRequest(new GetTrackingPointsResponse {
				Errors = this.ModelState.Values.SelectMany(v => v.Errors).Select(e => new Message {
					Code = e.Exception is null ? "" : e.Exception.GetType().Name,
					Description = e.ErrorMessage
				})
			});
		}

		try {
			GetTrackingPointsResponse response = await this._trackingService.GetTrackingPointsAsync(
				request,
				this.Request.Headers[HeaderNames.Authorization]
			);
			
			if (!response.Succeeded && response.Errors.Any(e =>
				    e.Code.Equals(GetTrackingPointsResponse.Error.InvalidToken.Code) ||
				    e.Code.Equals(GetTrackingPointsResponse.Error.Unauthorized.Code))) {
				return this.Unauthorized(response);
			}
			
			return this.Ok(response);
		}
		catch (Exception e) {
			this._logger.LogError(e, $"{nameof(this.GetTrackingPointsAsync)} threw an exception");
			return this.StatusCode(500, new GetTrackingPointsResponse {
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