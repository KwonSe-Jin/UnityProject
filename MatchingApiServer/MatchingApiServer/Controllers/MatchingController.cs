using Microsoft.AspNetCore.Mvc;
using MatchingApiServer.Services.Interface;
using MatchingApiServer.DTOs;

namespace MatchingApiServer.Controllers
{
	[ApiController]
	[Route("api/matching")]
	public class MatchingController : ControllerBase
	{
		private readonly IMatchingService _matchingService;
		private readonly ILogger<MatchingController> _logger;

		public MatchingController(IMatchingService matchingService, ILogger<MatchingController> logger)
		{
			_matchingService = matchingService;
			_logger = logger;
		}

		// 매칭 요청 (POST /api/matching/requestmatching)
		[HttpPost("requestmatching")]
		public async Task<IActionResult> RequestMatching([FromBody] MatchingReq request)
		{
			_logger.LogInformation($"매칭 요청 받음: {request.UserName}");
			var response = await _matchingService.RequestMatching(request);
			return Ok(response);
		}

		// 매칭 취소 (POST /api/matching/canclematching)
		[HttpPost("canclematching")]
		public async Task<IActionResult> CancelMatching([FromBody] CancelMatchingReq request)
		{
			_logger.LogInformation($"매칭 취소 요청 받음: {request.UserName}");
			var response = await _matchingService.CancelMatching(request);
			return Ok(response);
		}

		// 매칭 상태 확인 (POST /api/matching/checkmatching)
		[HttpPost("checkmatching")]
		public async Task<IActionResult> CheckMatching([FromBody] CheckMatchingReq request)
		{
			_logger.LogInformation($"매칭 상태 확인 요청 받음: {request.UserName}");
			var response = await _matchingService.CheckMatching(request);
			return Ok(response);
		}
	}
}
