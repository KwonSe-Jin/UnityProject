using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace GameServer
{
	public class RedisService
	{
		private readonly ConnectionMultiplexer _redis;
		private readonly IDatabase _db;
		private const string MatchQueueKey = "match_queue";

		public RedisService(string redisConnectionString)
		{
			_redis = ConnectionMultiplexer.Connect(redisConnectionString);
			_db = _redis.GetDatabase();
		}

		public async Task<long> GetMatchQueueLength()
		{
			return await _db.ListLengthAsync(MatchQueueKey);
		}

		public async Task<string?> PopFromMatchQueue()
		{
			return await _db.ListLeftPopAsync(MatchQueueKey);
		}

		public async Task SaveGameSession(string matchId, string player1, string player2, string serverIP, int serverPort)
		{
			string sessionKey = $"game_session:{matchId}";
			await _db.HashSetAsync(sessionKey, new HashEntry[]
			{
				new HashEntry("Player1", player1),
				new HashEntry("Player2", player2),
				new HashEntry("ServerIP", serverIP),
				new HashEntry("ServerPort", serverPort.ToString())
			});

			await _db.KeyExpireAsync(sessionKey, TimeSpan.FromMinutes(30));
		}
	}
}
