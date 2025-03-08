using MatchingApiServer.DTOs;
using MatchingApiServer.Enums;
using MatchingApiServer.Services.Interface;
using System.Collections.Concurrent;

namespace MatchingApiServer.Services
{
	public class MatchingService : IMatchingService
	{
		private static readonly ConcurrentDictionary<string, string> _matchQueue = new();
		private readonly ILogger<MatchingService> _logger;

		public MatchingService(ILogger<MatchingService> logger)
		{
			_logger = logger;
		}

		// 매칭 요청 처리
		public async Task<MatchingRes> RequestMatching(MatchingReq request)
		{
			_logger.LogInformation($"[매칭 요청] {request.UserName}");

			if (_matchQueue.ContainsKey(request.UserName))
			{
				return new MatchingRes { ErrorCode = ErrorCode.ALREADY_IN_QUEUE };
			}

			_matchQueue[request.UserName] = "Waiting";
			return new MatchingRes { ErrorCode = ErrorCode.SUCCESS };
		}

		// 매칭 취소 처리
		public async Task<CancelMatchingRes> CancelMatching(CancelMatchingReq request)
		{
			_logger.LogInformation($"[매칭 취소 요청] {request.UserName}");

			if (_matchQueue.TryRemove(request.UserName, out _))
			{
				return new CancelMatchingRes { ErrorCode = ErrorCode.SUCCESS };
			}

			return new CancelMatchingRes { ErrorCode = ErrorCode.NOT_FOUND };
		}

		// 매칭 상태 확인
		public async Task<CheckMatchingRes> CheckMatching(CheckMatchingReq request)
		{
			_logger.LogInformation($"[매칭 상태 확인] {request.UserName}");

			if (_matchQueue.TryGetValue(request.UserName, out var status))
			{
				return new CheckMatchingRes { ErrorCode = ErrorCode.SUCCESS, Status = status };
			}

			return new CheckMatchingRes { ErrorCode = ErrorCode.NOT_FOUND };
		}
	}
}
