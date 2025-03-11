using MatchingApiServer.DTOs;
using MatchingApiServer.Enums;
using MatchingApiServer.Services.Interface;
using System.Collections.Concurrent;

namespace MatchingApiServer.Services
{
	public class MatchingService : IMatchingService
	{
		private readonly RedisService _redisService;
		private readonly ILogger<MatchingService> _logger;

		public MatchingService(RedisService redisService, ILogger<MatchingService> logger)
		{
			_redisService = redisService;
			_logger = logger;
		}

		// 매칭 요청 처리 (Redis에 추가)
		public async Task<MatchingRes> RequestMatching(MatchingReq request)
		{
			_logger.LogInformation($"[매칭 요청] {request.UserName}");

			bool isAlreadyQueued = await _redisService.IsUserInMatchQueue(request.UserName);
			if (isAlreadyQueued)
			{
				return new MatchingRes { ErrorCode = ErrorCode.ALREADY_IN_QUEUE };
			}

			await _redisService.AddToMatchQueue(request.UserName);
			return new MatchingRes { ErrorCode = ErrorCode.SUCCESS };
		}

		// 매칭 취소 (Redis에서 제거)
		public async Task<CancelMatchingRes> CancelMatching(CancelMatchingReq request)
		{
			_logger.LogInformation($"[매칭 취소 요청] {request.UserName}");

			bool removed = await _redisService.RemoveFromMatchQueue(request.UserName);
			if (removed)
			{
				return new CancelMatchingRes { ErrorCode = ErrorCode.SUCCESS };
			}

			return new CancelMatchingRes { ErrorCode = ErrorCode.NOT_FOUND };
		}

		// 매칭 상태 확인 (Redis에서 조회)
		public async Task<CheckMatchingRes> CheckMatching(CheckMatchingReq request)
		{
			_logger.LogInformation($"[매칭 상태 확인] {request.UserName}");

			bool isInQueue = await _redisService.IsUserInMatchQueue(request.UserName);
			if (isInQueue)
			{
				return new CheckMatchingRes { ErrorCode = ErrorCode.SUCCESS, Status = "Waiting" };
			}

			return new CheckMatchingRes { ErrorCode = ErrorCode.NOT_FOUND };
		}

		// 매칭된 유저 가져오기 (대기열에서 꺼내기)
		public async Task<string?> GetMatchedUser()
		{
			return await _redisService.PopFromMatchQueue();
		}
	}
}
