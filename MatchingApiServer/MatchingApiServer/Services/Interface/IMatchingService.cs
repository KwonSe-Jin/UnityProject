using MatchingApiServer.DTOs;

namespace MatchingApiServer.Services.Interface
{
	public interface IMatchingService
	{
		Task<MatchingRes> RequestMatching(MatchingReq req);
		Task<CancelMatchingRes> CancelMatching(CancelMatchingReq req);
		Task<CheckMatchingRes> CheckMatching(CheckMatchingReq req);
	}
}
