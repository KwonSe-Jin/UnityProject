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

			// Redis 및 GameServer 직접 생성
			RedisService redisService = new RedisService(redisConnectionString);
			GameServer gameServer = new GameServer(redisService);

			// 매칭 루프 실행
			await gameServer.CheckAndStartMatch();
		}
	}
}
