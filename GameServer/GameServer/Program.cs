using StackExchange.Redis;
using System;
using System.Threading.Tasks;


namespace GameServer
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("GameServer Starting...");

			string redisConnectionString = "localhost:6379"; // Redis 연결 정보

			var redis = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
			var db = redis.GetDatabase();

			// 방 관리 매니저
			var roomManager = new RoomManager();

			// 매칭 감시 스레드 실행
			var matchMaker = new GameServerMatchMaker(db, roomManager, "127.0.0.1", 9000);
			var matchTask = matchMaker.StartMatchingLoop(CancellationToken.None);

			// WebSocket 서버 실행 (다음 단계에서 추가할 부분)
			await WebSocketHost.Start(roomManager);

			await matchTask; // 이 줄은 사실상 서버가 종료되지 않도록 하기 위함
		}
	}
}
