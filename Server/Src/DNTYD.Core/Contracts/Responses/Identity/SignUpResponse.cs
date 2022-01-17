namespace DNTYD.Core.Contracts.Responses.Identity;

public class SignUpResponse : BaseResponse {
	public static class Message {
		public static readonly DNTYD.Core.ValueObjects.Message SignedUp = new() {
			Code = nameof(SignedUp),
			Description = "You have been signed up"
		};
	}
}