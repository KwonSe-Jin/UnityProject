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

			// 초기 캐릭터 데이터 추가 
			modelBuilder.Entity<Character>().HasData( 
				new Character { CharacterId = 1, Name = "전사", Description = "근접 캐릭터", Price = 500, IsOnSale = true },
				new Character { CharacterId = 2, Name = "마법사", Description = "마법 캐릭터", Price = 600, IsOnSale = true },
				new Character { CharacterId = 3, Name = "궁수", Description = "활 캐릭터", Price = 550, IsOnSale = true }
			);
		}
	}
}
