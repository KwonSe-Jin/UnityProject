using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace WebAPIServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")] // API 경로: /api/player
	public class PlayerController : ControllerBase
	{
		private readonly DataBaseContext _context;

		public PlayerController(DataBaseContext context)
		{
			_context = context;
		}

		// 모든 플레이어 조회 (GET /api/player)
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
		{
			return await _context.Players.ToListAsync();
		}

		// 특정 플레이어 조회 (GET /api/player/{id})
		[HttpGet("{id}")]
		public async Task<ActionResult<Player>> GetPlayer(int id)
		{
			var player = await _context.Players.FindAsync(id);
			if (player == null)
			{
				return NotFound(); // 404 에러 반환
			}
			return player;
		}

		// 새로운 플레이어 추가 (POST /api/player)
		[HttpPost]
		public async Task<ActionResult<Player>> PostPlayer(Player player)
		{
			_context.Players.Add(player);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetPlayer), new { id = player.PlayerId }, player);
		}
	}
}
