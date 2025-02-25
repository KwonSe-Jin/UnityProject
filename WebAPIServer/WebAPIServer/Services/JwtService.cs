using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace WebAPIServer.Services
{
	public class JwtService
	{
		private readonly string _secretKey;
		private readonly string _issuer;
		private readonly string _audience;
		private readonly int _expirationMinutes;

		public JwtService(IConfiguration configuration)
		{
			var jwtSettings = configuration.GetSection("JwtSettings");
			_secretKey = jwtSettings["SecretKey"];
			_issuer = jwtSettings["Issuer"];
			_audience = jwtSettings["Audience"];
			_expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"]);
		}

		public string GenerateToken(int playerId, string playerName)
		{
			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, playerId.ToString()),
			new Claim(JwtRegisteredClaimNames.UniqueName, playerName),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				_issuer,
				_audience,
				claims,
				expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
