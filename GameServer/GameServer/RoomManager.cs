using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
	public class RoomManager
	{
		private readonly Dictionary<string, GameRoom> _rooms = new();

		public void CreateRoom(string player1, string player2, string token)
		{
			var room = new GameRoom
			{
				Player1 = player1,
				Player2 = player2,
				RoomToken = token
			};

			_rooms[token] = room;
		}

		public GameRoom? GetRoomByToken(string token)
		{
			_rooms.TryGetValue(token, out var room);
			return room;
		}
	}
	public class GameRoom
	{
		public string RoomToken { get; set; }
		public string Player1 { get; set; }
		public string Player2 { get; set; }
		public WebSocket? Socket1 { get; set; }
		public WebSocket? Socket2 { get; set; }

		public bool IsFull => Socket1 != null && Socket2 != null;
	}
}
