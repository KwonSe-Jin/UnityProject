using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebAPIServer.Enums; // ErrorCode 사용
using WebAPIServer.DTOs; // CancelMatchingRes 사용
using WebAPIServer.Services.Interface;
using WebAPIServer.Services;

public class MatchingService : IMatchingService
{
	private readonly MatchingRedisService _redisService;
	private readonly ILogger<MatchingService> _logger;

	public MatchingService(MatchingRedisService redisService, ILogger<MatchingService> logger)
	{
		_redisService = redisService;
		_logger = logger;
	}

	// 매칭 요청 처리 (Redis에 유저 추가)
	public async Task<MatchingRes> RequestMatching(MatchingReq request)
	{
		_logger.LogInformation($"[매칭 요청] {request.UserName}");

		// 이미 매칭 대기열에 존재하는지 확인
		bool isAlreadyQueued = await _redisService.IsUserInMatchQueue(request.UserName);
		if (isAlreadyQueued)
		{
			// 이미 대기 중이라면 에러 반환
			return new MatchingRes { ErrorCode = ErrorCode.ALREADY_IN_QUEUE };
		}

		// 대기열에 추가
		await _redisService.AddToMatchQueue(request.UserName);
		return new MatchingRes { ErrorCode = ErrorCode.SUCCESS };
	}

	// 매칭 취소 요청 처리 (Redis 대기열에서 제거)
	public async Task<CancelMatchingRes> CancelMatching(CancelMatchingReq request)
	{
		_logger.LogInformation($"[매칭 취소 요청] {request.UserName}");

		// 매칭 대기열에서 유저 제거 시도
		bool removed = await _redisService.RemoveFromMatchQueue(request.UserName);
		if (removed)
		{
			// 제거
			return new CancelMatchingRes { ErrorCode = ErrorCode.SUCCESS };
		}

		// 제거 실패  / 대기열에 없음
		return new CancelMatchingRes { ErrorCode = ErrorCode.NOT_FOUND };
	}

	// 매칭 상태 확인 (현재 대기 중인지, 매칭 완료됐는지 조회)
	public async Task<CheckMatchingRes> CheckMatching(CheckMatchingReq request)
	{
		_logger.LogInformation($"[매칭 상태 확인] {request.UserName}");

		// 유저가 대기열에 있는지 확인
		bool isInQueue = await _redisService.IsUserInMatchQueue(request.UserName);
		if (isInQueue)
		{
			// 아직 매칭 중  - 대기열에 있음
			return new CheckMatchingRes { ErrorCode = ErrorCode.SUCCESS, Status = "Waiting" };
		}

		// 매칭이 완료된 경우 방 정보 조회
		var roomInfo = await _redisService.GetMatchedRoomInfo(request.UserName);
		if (roomInfo != null)
		{
			// 매칭 완료된 경우 방 정보와 함께 반환
			return new CheckMatchingRes
			{
				ErrorCode = ErrorCode.SUCCESS,
				Status = "Matched",
				RoomIP = roomInfo.IP,
				RoomPort = roomInfo.Port,
				RoomToken = roomInfo.RoomToken
			};
		}

		// 대기열에도 없고 매칭된 방 정보도 없는 경우
		return new CheckMatchingRes { ErrorCode = ErrorCode.NOT_FOUND };
	}


	// 매칭된 유저 가져오기 (대기열에서 꺼내기)
	public async Task<string?> GetMatchedUser()
	{
		return await _redisService.PopFromMatchQueue();
	}
}
