using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebAPIServer.Enums; // ErrorCode 사용
using WebAPIServer.DTOs; // CancelMatchingRes 사용
using WebAPIServer.Services.Interface;

public class MatchingService : IMatchingService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<MatchingService> _logger;

	public MatchingService(ILogger<MatchingService> logger, IConfiguration configuration)
	{
		_logger = logger;
		_httpClient = new HttpClient { BaseAddress = new Uri(configuration["MatchServerUrl"]!) };
	}

	public async Task<MatchingRes> RequestMatching(MatchingReq req)
	{
		try
		{
			var httpResponse = await _httpClient.PostAsJsonAsync("api/requestmatching", req);
			if (httpResponse.StatusCode != HttpStatusCode.OK)
			{
				_logger.LogError("RequestMatching failed");
				return new MatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
			}

			var response = await httpResponse.Content.ReadFromJsonAsync<MatchingRes>();
			return response ?? new MatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
		}
		catch (Exception e)
		{
			_logger.LogError(e, "RequestMatching failed");
			return new MatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
		}
	}

	public async Task<CancelMatchingRes> CancelMatching(CancelMatchingReq req)
	{
		try
		{
			var httpResponse = await _httpClient.PostAsJsonAsync("api/canclematching", req);
			if (httpResponse.StatusCode != HttpStatusCode.OK)
			{
				_logger.LogError("CancleMatching failed");
				return new CancelMatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
			}

			var response = await httpResponse.Content.ReadFromJsonAsync<CancelMatchingRes>();
			return response ?? new CancelMatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
		}
		catch (Exception e)
		{
			_logger.LogError(e, "CancleMatching failed");
			return new CancelMatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
		}
	}

	public async Task<CheckMatchingRes> CheckMatching(CheckMatchingReq req)
	{
		try
		{
			var httpResponse = await _httpClient.PostAsJsonAsync("api/checkmatching", req);
			if (httpResponse.StatusCode != HttpStatusCode.OK)
			{
				_logger.LogError("CheckMatching failed");
				return new CheckMatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
			}

			var response = await httpResponse.Content.ReadFromJsonAsync<CheckMatchingRes>();
			return response ?? new CheckMatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
		}
		catch (Exception e)
		{
			_logger.LogError(e, "CheckMatching failed");
			return new CheckMatchingRes() { ErrorCode = ErrorCode.MATCHING_SERVER_ERROR };
		}
	}
}
