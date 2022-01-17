﻿namespace DNTYD.Core.Contracts.Responses.Tracking;

public class AddTrackingPointResponse : BaseResponse {
	public static class Message {
		public static readonly DNTYD.Core.ValueObjects.Message TrackingPointAdded = new() {
			Code = nameof(TrackingPointAdded),
			Description = "The tracking point was added to the user profile"
		};
	}
	
	public static class Error {
		public static readonly DNTYD.Core.ValueObjects.Message InvalidToken = new() {
			Code = nameof(InvalidToken),
			Description = "The token is invalid due to wrong or missing information"
		};
		
		public static readonly DNTYD.Core.ValueObjects.Message Unauthorized = new() {
			Code = nameof(Unauthorized),
			Description = "The user is unauthorized the add a tracking point"
		};
		
		public static readonly DNTYD.Core.ValueObjects.Message InternalError = new () {
			Code = nameof(InternalError),
			Description = "Putting the gps tracking point failed due to an internal server error"
		};
	}
}