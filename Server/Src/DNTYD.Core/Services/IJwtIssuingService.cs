using System.Security.Claims;

namespace DNTYD.Core.Services; 

public interface IJwtIssuingService {
	string IssueToken(string secret, string sub, string email, TimeSpan expiringTime, Claim[] additionalClaims);
	string? GetSubFromToken(string token);
}