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
				new Claim(JwtRegisteredClaimNames.Sub, playerId.ToString()),   // 사용자 ID
                new Claim(ClaimTypes.Name, playerName),  // 사용자 이름
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT 고유 ID
                new Claim(ClaimTypes.Role, "Player")  // `[Authorize(Roles = "Player")]` 사용 가능
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

		public ClaimsPrincipal? ValidateToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_secretKey);

			try
			{
				var parameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true, // 1. 서명 검증 (토큰 변조 확인)

					IssuerSigningKey = new SymmetricSecurityKey(key),

					ValidateIssuer = true, // 2. 발급자 확인 (iss 필드 체크)

					ValidIssuer = _issuer,

					ValidateAudience = true, // 3. 대상 확인 (aud 필드 체크)

					ValidAudience = _audience,

					ValidateLifetime = true, // 4. 만료 시간 확인 (exp 필드 체크)

					ClockSkew = TimeSpan.Zero
				};

				return tokenHandler.ValidateToken(token, parameters, out _);
			}
			catch (SecurityTokenExpiredException)
			{
				Console.WriteLine(" JWT 토큰이 만료되었습니다.");
				return null;
			}
			catch (SecurityTokenInvalidSignatureException)
			{
				Console.WriteLine(" JWT 서명이 유효하지 않습니다.");
				return null;
			}
			catch (Exception ex)
			{
				Console.WriteLine($" JWT 검증 중 오류 발생: {ex.Message}");
				return null;
			}
		}
	}
}
