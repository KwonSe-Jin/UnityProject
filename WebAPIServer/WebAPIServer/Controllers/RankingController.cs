using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIServer.Services;

namespace WebAPIServer.Controllers
{
	[ApiController]
	[Route("api/ranking")]
	public class RankingController : ControllerBase
	{
		private readonly RedisService _redisService;
		private readonly JwtService _jwtService;
		public RankingController(RedisService redisService, JwtService jwtService)
		{
			_redisService = redisService;
			_jwtService = jwtService;
		}
		// 점수 업데이트 API (POST /api/ranking)
		[Authorize]
		[HttpPost]
		public IActionResult UpdatePlayerScore([FromHeader] string authorization, [FromBody] PlayerScoreRequest request)
		{
			if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
			{
				return Unauthorized(new { message = "JWT 토큰이 필요합니다." });
			}
			var token = authorization.Substring("Bearer ".Length).Trim();
			var claimsPrincipal = _jwtService.ValidateToken(token);

			if (claimsPrincipal == null)
			{
				return Unauthorized(new { message = "유효하지 않은 JWT 토큰입니다." });
			}

			var playerName = claimsPrincipal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

			if (string.IsNullOrEmpty(playerName))
			{
				return Unauthorized(new { message = "JWT에서 플레이어 정보를 찾을 수 없습니다." });
			}

			_redisService.UpdatePlayerScore(playerName, request.Score);

			return Ok(new { message = $"플레이어 {playerName} 점수 업데이트 완료!", PlayerName = playerName, Score = request.Score });
		}

		// 상위 랭킹 조회 API (GET /api/ranking)
		[HttpGet]
		public IActionResult GetTopPlayers([FromQuery] int count = 10)
		{
			var topPlayers = _redisService.GetTopPlayers(count);
			return Ok(topPlayers);
		}

		// 특정 날짜의 랭킹 조회 API (GET /api/ranking/date)
		[HttpGet("date")]
		public IActionResult GetTopPlayersByDate([FromQuery] string date, [FromQuery] int count = 10)
		{
			// 입력 예시 data = 2025-02-27
			var topPlayers = _redisService.GetTopPlayersByDate(date, count);
			return Ok(topPlayers);
		}
	}

	public class PlayerScoreRequest
	{
		public int Score { get; set; }
	}
}
