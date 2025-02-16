using StackExchange.Redis;

namespace WebAPIServer.Services
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

		// 플레이어 점수 저장 (Sorted Set 사용)
		public void UpdatePlayerScore(string playerName, int score)
		{
			_db.SortedSetAdd("leaderboard", playerName, score);
		}

		// 상위 랭킹 조회
		public List<(string, double)> GetTopPlayers(int count = 10)
		{
			var topPlayers = _db.SortedSetRangeByRankWithScores("leaderboard", 0, count - 1, Order.Descending);

			// 디버깅 로그 추가
			Console.WriteLine($"[Redis] Leaderboard Count: {topPlayers.Length}");
			return topPlayers.Select(x => (x.Element.ToString(), x.Score)).ToList();
		}
	}
}
