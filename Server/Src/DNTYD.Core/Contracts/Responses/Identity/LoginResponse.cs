namespace DNTYD.Core.Contracts.Responses.Identity; 

public class LoginResponse : BaseResponse {
	public string Token { get; set; } = "";

	public static class Message {
		public static readonly DNTYD.Core.ValueObjects.Message LoggedIn = new() {
			Code = nameof(LoggedIn),
			Description = "User logged in successfully"
		};
	}
	
	public static class Error {
		public static readonly DNTYD.Core.ValueObjects.Message UsernameOrEmailNotRegistered = new() {
			Code = nameof(UsernameOrEmailNotRegistered),
			Description = "The username or email is not registered"
		};
		
		public static readonly DNTYD.Core.ValueObjects.Message PasswordNotCorrect = new() {
			Code = nameof(PasswordNotCorrect),
			Description = "The password is not correct"
		};
		
		public static readonly DNTYD.Core.ValueObjects.Message InternalError = new () {
			Code = nameof(InternalError),
			Description = "The login process failed due to an internal server error"
		};
	}
}