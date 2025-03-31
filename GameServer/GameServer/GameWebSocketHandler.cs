using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	public class GameWebSocketHandler
	{
		private readonly RoomManager _roomManager;

		public GameWebSocketHandler(RoomManager roomManager)
		{
			_roomManager = roomManager;
		}

		public async Task HandleConnection(HttpContext context, WebSocket socket)
		{
			var buffer = new byte[1024];
			var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			string token = Encoding.UTF8.GetString(buffer, 0, result.Count);

			var room = _roomManager.GetRoomByToken(token);
			if (room == null)
			{
				await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid Token", CancellationToken.None);
				return;
			}

			if (room.Socket1 == null)
				room.Socket1 = socket;
			else if (room.Socket2 == null)
				room.Socket2 = socket;
			else
			{
				await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Room Full", CancellationToken.None);
				return;
			}

			Console.WriteLine($"[WebSocket 연결 완료] Token: {token}, 방 상태: {(room.IsFull ? "Full" : "Waiting")}");

			if (room.IsFull)
			{
				Console.WriteLine($"[게임 시작] {room.Player1} vs {room.Player2}");
			}
		}
	}
}
