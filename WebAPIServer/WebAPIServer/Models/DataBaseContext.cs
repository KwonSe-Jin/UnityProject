using Microsoft.EntityFrameworkCore;

namespace WebAPIServer.Models
{
	public class DataBaseContext : DbContext
	{
		public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

		public DbSet<Player> Players { get; set; } // 기존 테이블
		public DbSet<Character> Characters { get; set; }
		public DbSet<UserCharacter> UserCharacters { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Player>()
				.HasIndex(p => p.PlayerName)
				.IsUnique(); // PlayerName을 유니크 값으로 설정 중복 키 삽입 불가
		}
	}
}
