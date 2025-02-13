using Microsoft.EntityFrameworkCore;

namespace WebServer.Models
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }  // 테이블 이름: "Users"
	}

	public class User  
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Email { get; set; }
	}
}
