using StackExchange.Redis;

namespace MatchingApiServer.Services
{
	public class RedisService
	{
		private readonly ConnectionMultiplexer _redis;
		private readonly IDatabase _db;

		public RedisService()
		{
			_redis = ConnectionMultiplexer.Connect("localhost:6379"); // Redis 서버 주소
			_db = _redis.GetDatabase();
		}

		public async Task AddToMatchQueue(string userName)
		{
			await _db.SetAddAsync("match_queue", userName);
		}

		public async Task<bool> RemoveFromMatchQueue(string userName)
		{
			return await _db.SetRemoveAsync("match_queue", userName);
		}

		public async Task<bool> IsUserInMatchQueue(string userName)
		{
			return await _db.SetContainsAsync("match_queue", userName);
		}
	}
}
