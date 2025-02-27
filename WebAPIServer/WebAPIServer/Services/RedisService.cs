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
			string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
			string key = $"ranking:{today}"; // 날짜별 키 생성

			_db.SortedSetAdd(key, playerName, score);
		}

		// 상위 랭킹 조회
		public List<RankingEntry> GetTopPlayers(int count = 10)
		{
			string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
			string key = $"ranking:{today}"; // 오늘 날짜의 랭킹 데이터 가져오기

			var topPlayers = _db.SortedSetRangeByScoreWithScores(key, double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending, 0, count);

			Console.WriteLine($"[Redis] {key} Leaderboard Count: {topPlayers.Length}");

			return topPlayers.Select(x => new RankingEntry
			{
				PlayerName = x.Element.ToString(),
				Score = x.Score
			}).ToList();
		}
		public List<RankingEntry> GetTopPlayersByDate(string date, int count = 10)
		{
			string key = $"ranking:{date}"; // 사용자가 요청한 날짜의 랭킹 조회

			var topPlayers = _db.SortedSetRangeByScoreWithScores(key, double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending, 0, count);

			return topPlayers.Select(x => new RankingEntry
			{
				PlayerName = x.Element.ToString(),
				Score = x.Score
			}).ToList();
		}

		public class RankingEntry
		{
			public string? PlayerName { get; set; }
			public double Score { get; set; }
		}
	}
}
