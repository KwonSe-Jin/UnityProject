using Microsoft.AspNetCore.Mvc;
using WebAPIServer.Services;

namespace WebAPIServer.Controllers
{
	[ApiController]
	[Route("api/ranking")]
	public class RankingController : ControllerBase
	{
		private readonly RedisService _redisService;

		public RankingController(RedisService redisService)
		{
			_redisService = redisService;
		}

		// 점수 업데이트 API (POST /api/ranking)
		[HttpPost]
		public IActionResult UpdatePlayerScore([FromBody] PlayerScoreRequest request)
		{
			_redisService.UpdatePlayerScore(request.PlayerName, request.Score);
			return Ok($"Player {request.PlayerName} score updated!");
		}

		// 상위 랭킹 조회 API (GET /api/ranking)
		[HttpGet]
		public IActionResult GetTopPlayers([FromQuery] int count = 10)
		{
			var topPlayers = _redisService.GetTopPlayers(count);
			return Ok(topPlayers);
		}
	}

	public class PlayerScoreRequest
	{
		public string? PlayerName { get; set; }
		public int Score { get; set; }
	}
}
