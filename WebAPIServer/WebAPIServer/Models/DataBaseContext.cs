using Microsoft.EntityFrameworkCore;

namespace WebAPIServer.Models
{
	public class DataBaseContext : DbContext
	{
		public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

		public DbSet<Player> Players { get; set; } // 기존 테이블

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Player>()
				.HasIndex(p => p.PlayerName)
				.IsUnique(); // PlayerName을 유니크 값으로 설정
		}
	}
}
