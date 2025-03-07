using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPIServer.DTOs;
using WebAPIServer.Services.Interface;

namespace WebAPIServer.Controllers
{
	[Authorize] // JWT 인증된 유저만 매칭 가능
	[ApiController]
	[Route("api/match")]
	public class MatchingController : ControllerBase
	{
		private readonly IMatchingService _matchService;

		public MatchingController(IMatchingService matchService)
		{
			_matchService = matchService;
		}

		// 매칭 요청 (POST /api/match/request)
		[HttpPost("request")]
		public async Task<IActionResult> RequestMatch()
		{
			var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

			if (string.IsNullOrEmpty(userName))
			{
				return Unauthorized(new { message = "유효하지 않은 사용자입니다." });
			}

			var matchRequest = new MatchingReq { UserName = userName };
			var response = await _matchService.RequestMatching(matchRequest);

			return Ok(response);
		}

		// 매칭 취소 (POST /api/match/cancel)
		[HttpPost("cancel")]
		public async Task<IActionResult> CancelMatch()
		{
			var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

			if (string.IsNullOrEmpty(username))
			{
				return Unauthorized(new { message = "유효하지 않은 사용자입니다." });
			}

			var cancelRequest = new CancelMatchingReq { UserName = username };
			var response = await _matchService.CancelMatching(cancelRequest);

			return Ok(response);
		}

		// 매칭 상태 확인 (POST /api/match/status)
		[HttpPost("status")]
		public async Task<IActionResult> CheckMatchStatus()
		{
			var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

			if (string.IsNullOrEmpty(username))
			{
				return Unauthorized(new { message = "유효하지 않은 사용자입니다." });
			}

			var checkRequest = new CheckMatchingReq { UserName = username };
			var response = await _matchService.CheckMatching(checkRequest);

			return Ok(response);
		}
	}
}
