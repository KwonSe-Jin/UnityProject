using StackExchange.Redis;

namespace WebAPIServer.Services
{
	public class MatchingRedisService
	{
		private readonly ConnectionMultiplexer _redis;
		private readonly IDatabase _db;
		private const string MatchQueueKey = "match_queue";

		public MatchingRedisService()
		{
			_redis = ConnectionMultiplexer.Connect("localhost:6379");
			_db = _redis.GetDatabase();
		}

		public async Task AddToMatchQueue(string userName) =>
			await _db.ListRightPushAsync(MatchQueueKey, userName);

		public async Task<bool> RemoveFromMatchQueue(string userName) =>
			await _db.ListRemoveAsync(MatchQueueKey, userName) > 0;

		public async Task<string?> PopFromMatchQueue() =>
			await _db.ListLeftPopAsync(MatchQueueKey);

		public async Task<bool> IsUserInMatchQueue(string userName)
		{
			var queue = await _db.ListRangeAsync(MatchQueueKey, 0, -1);
			return queue.Contains(userName);
		}

		public async Task<string[]> GetMatchQueue()
		{
			var queue = await _db.ListRangeAsync(MatchQueueKey, 0, -1);
			return Array.ConvertAll(queue, x => x.ToString());
		}
	}
}

