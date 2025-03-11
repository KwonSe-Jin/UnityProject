using StackExchange.Redis;

namespace MatchingApiServer.Services
{
	public class RedisService
	{
		private readonly ConnectionMultiplexer _redis;
		private readonly IDatabase _db;
		private const string MatchQueueKey = "match_queue"; // Redis 키 값
		public RedisService()
		{
			_redis = ConnectionMultiplexer.Connect("localhost:6379"); // Redis 서버 주소
			_db = _redis.GetDatabase();
		}

		public async Task AddToMatchQueue(string userName)
		{
			await _db.ListRightPushAsync(MatchQueueKey, userName);
		}

		public async Task<bool> RemoveFromMatchQueue(string userName)
		{
			long removed = await _db.ListRemoveAsync(MatchQueueKey, userName);
			return removed > 0;
		}

		// 매칭 대기열에서 가장 먼저 들어온 유저 꺼내기
		public async Task<string?> PopFromMatchQueue()
		{
			return await _db.ListLeftPopAsync(MatchQueueKey);
		}

		// 매칭 대기열에 특정 유저가 있는지 확인
		public async Task<bool> IsUserInMatchQueue(string userName)
		{
			var queue = await _db.ListRangeAsync(MatchQueueKey, 0, -1);
			return queue.Contains(userName);
		}

		// 현재 매칭 대기열 조회
		public async Task<string[]> GetMatchQueue()
		{
			var queue = await _db.ListRangeAsync(MatchQueueKey, 0, -1);
			return Array.ConvertAll(queue, x => x.ToString());
		}
	}
}
