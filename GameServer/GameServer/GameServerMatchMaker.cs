using StackExchange.Redis;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
	public class GameServerMatchMaker
	{
		private readonly IDatabase _db;
		private readonly RoomManager _roomManager;
		private readonly string _ip;
		private readonly int _fixedPort;

		public GameServerMatchMaker(IDatabase db, RoomManager manager, string ip, int fixedPort)
		{
			_db = db;
			_roomManager = manager;
			_ip = ip;
			_fixedPort = fixedPort; // 포트는 고정값 사용 (예: 9000)
		}

		public async Task StartMatchingLoop(CancellationToken token)
		{
			Console.WriteLine("[매칭 스레드] 시작");

			while (!token.IsCancellationRequested)
			{
				var queue = await _db.ListRangeAsync("match_queue", 0, -1);

				if (queue.Length >= 2)
				{
					string? player1 = await _db.ListLeftPopAsync("match_queue");
					string? player2 = await _db.ListLeftPopAsync("match_queue");

					if (!string.IsNullOrEmpty(player1) && !string.IsNullOrEmpty(player2))
					{
						string tokenStr = Guid.NewGuid().ToString();

						_roomManager.CreateRoom(player1, player2, tokenStr);

						var roomInfo = new HashEntry[]
						{
							new("ip", _ip),
							new("port", _fixedPort.ToString()), // 고정 포트
							new("roomToken", tokenStr),
							new("players", $"{player1},{player2}")
						};

						await _db.HashSetAsync($"matched_room:{player1}", roomInfo);
						await _db.HashSetAsync($"matched_room:{player2}", roomInfo);
						await _db.KeyExpireAsync($"matched_room:{player1}", TimeSpan.FromMinutes(2));
						await _db.KeyExpireAsync($"matched_room:{player2}", TimeSpan.FromMinutes(2));

						Console.WriteLine($"[매칭 완료] {player1} vs {player2} @ {_ip}:{_fixedPort}");
					}
				}

				await Task.Delay(500); // 0.5초마다 매칭 검사
			}
		}
	}
}
