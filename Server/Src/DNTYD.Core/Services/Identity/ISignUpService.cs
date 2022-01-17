using DNTYD.Core.Contracts.Requests.Identity;
using DNTYD.Core.Contracts.Responses.Identity;

namespace DNTYD.Core.Services.Identity; 

public interface ISignUpService {
	Task<SignUpResponse> SignUpUserAsync(SignUpRequest request);
}