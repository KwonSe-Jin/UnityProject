using Microsoft.EntityFrameworkCore;

namespace WebAPIServer.Models
{
	public class DataBaseContext : DbContext
	{
		public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

		public DbSet<Player> Players { get; set; } // 기존 테이블
	}
}
