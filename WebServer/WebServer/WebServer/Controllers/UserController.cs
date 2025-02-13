using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]  // URL: /api/user
	public class UserController : ControllerBase
	{
		private readonly AppDbContext _context;

		public UserController(AppDbContext context)
		{
			_context = context;
		}

		// 모든 유저 조회 (GET /api/user)
		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetUsers()
		{
			return await _context.Users.ToListAsync();
		}

		// 새로운 유저 추가 (POST /api/user)
		[HttpPost]
		public async Task<ActionResult<User>> PostUser(User user)
		{
			_context.Users.Add(user);
			await _context.SaveChangesAsync().ConfigureAwait(false); // 비동기 문제 방지
			return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
		}
	}
}
