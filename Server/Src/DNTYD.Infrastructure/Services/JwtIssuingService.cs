using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using DNTYD.Core.Services;

using Microsoft.IdentityModel.Tokens;

namespace DNTYD.Infrastructure.Services; 

public class JwtIssuingService : IJwtIssuingService {
	public string IssueToken(string secret, string sub, string email, TimeSpan expiringTime, Claim[] claims) {
		byte[] secretBytes = Encoding.ASCII.GetBytes(secret);
		JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

		SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor() {
			Subject = new ClaimsIdentity(new[] {
				new Claim(JwtRegisteredClaimNames.Sub, sub),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Email, email)
			}.Concat(claims)),
			Expires = DateTime.UtcNow + expiringTime,
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretBytes), SecurityAlgorithms.HmacSha256Signature)
		};

		SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public string? GetSubFromToken(string token) {
		JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
		SecurityToken securityToken = tokenHandler.ReadToken(token);

		if (securityToken is not JwtSecurityToken jwtSecurityToken)
			return null;

		return jwtSecurityToken.Claims.FirstOrDefault(c => c.Type.Equals(JwtRegisteredClaimNames.Sub))?.Value;
	}
}