using WebAPIServer.Enums;

namespace WebAPIServer.DTOs
{
	public class MatchingReq
	{
		public string UserName { get; set; } = string.Empty;
	}
	public class MatchingRes
	{
		public ErrorCode ErrorCode { get; set; } = ErrorCode.SUCCESS;
		public string MatchId { get; set; } = string.Empty; // 매칭 ID
		public bool IsMatched { get; set; } = false; // 매칭 완료 여부
	}

	public class CancelMatchingReq
	{
		public string UserName { get; set; } = string.Empty;
	}

	public class CancelMatchingRes
	{
		public ErrorCode ErrorCode { get; set; } = ErrorCode.SUCCESS;
		public bool IsCanceled { get; set; } = false; // 취소 성공 여부
	}

	public class CheckMatchingReq
	{
		public string UserName { get; set; } = string.Empty;
	}

	public class CheckMatchingRes
	{
		public ErrorCode ErrorCode { get; set; }
		public string Status { get; set; } = "";
		public string? RoomIP { get; set; }
		public int? RoomPort { get; set; }
		public string? RoomToken { get; set; }
	}
	public class MatchedRoomInfo
	{
		public string IP { get; set; } = "";
		public int Port { get; set; }
		public string RoomToken { get; set; } = "";
	}
}
