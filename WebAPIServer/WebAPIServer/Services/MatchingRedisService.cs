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

		public async Task<MatchedRoomInfo?> GetMatchedRoomInfo(string userName)
		{
			string key = $"matched_room:{userName}";
			if (!await _db.KeyExistsAsync(key)) return null;

			var entries = await _db.HashGetAllAsync(key);
			if (entries.Length == 0) return null;

			var dict = entries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());

			return new MatchedRoomInfo
			{
				IP = dict["ip"],
				Port = int.Parse(dict["port"]),
				RoomToken = dict["roomToken"]
			};
		}

		public class MatchedRoomInfo
		{
			public string IP { get; set; } = "";
			public int Port { get; set; }
			public string RoomToken { get; set; } = "";
		}
	}
}

