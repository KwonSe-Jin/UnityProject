using System;
using System.Threading.Tasks;

namespace GameServer
{
	public class GameServer
	{
		private readonly RedisService _redisService;
		private const string ServerIP = "127.0.0.1"; // 게임 서버 IP
		private const int ServerPort = 7777; // 게임 서버 포트

		public GameServer(RedisService redisService)
		{
			_redisService = redisService;
		}

		public async Task CheckAndStartMatch()
		{
			Console.WriteLine("GameServer started. Checking match queue...");

			while (true) // 주기적으로 Redis 체크
			{
				var queueLength = await _redisService.GetMatchQueueLength();

				if (queueLength >= 2)
				{
					var player1 = await _redisService.PopFromMatchQueue();
					var player2 = await _redisService.PopFromMatchQueue();

					if (player1 != null && player2 != null)
					{
						string matchId = Guid.NewGuid().ToString(); // 랜덤한 매칭 ID 생성

						// Redis에 게임 세션 정보 저장
						await _redisService.SaveGameSession(matchId, player1, player2, ServerIP, ServerPort);

						Console.WriteLine($"매칭 완료! {player1} vs {player2} (서버: {ServerIP}:{ServerPort}) → Match ID: {matchId}");
					}
				}

				await Task.Delay(1000); // 1초마다 매칭 확인
			}
		}
	}
}
