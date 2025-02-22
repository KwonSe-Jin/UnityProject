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
				return NotFound(new { message = "해당 ID의 플레이어를 찾을 수 없습니다." });
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

		[HttpPost("register")]
		public async Task<ActionResult> Register(PlayerRegisterRequest request)
		{
			// 중복 체크
			if (await _context.Players.AnyAsync(p => p.PlayerName == request.PlayerName))
			{
				return BadRequest(new { message = "이미 존재하는 플레이어 이름입니다!" });
			}

			var newPlayer = new Player
			{
				PlayerName = request.PlayerName
			};
			newPlayer.SetPassword(request.Password); // 비밀번호 해싱 적용

			_context.Players.Add(newPlayer);
			await _context.SaveChangesAsync();

			return Ok(new { message = "회원가입 성공!" });
		}

		[HttpPost("login")]
		public async Task<ActionResult> Login(PlayerLoginRequest request)
		{
			var player = await _context.Players.FirstOrDefaultAsync(p => p.PlayerName == request.PlayerName);
			if (player == null || !player.VerifyPassword(request.Password))
			{
				return Unauthorized(new { message = "로그인 실패! 아이디 또는 비밀번호를 확인하세요." });
			}

			//  할일 JWT 발급
			return Ok(new { message = "로그인 성공!", playerId = player.PlayerId });
		}

	}
	//  회원가입 요청 모델
	public class PlayerRegisterRequest
	{
		public string PlayerName { get; set; }
		public string Password { get; set; }
	}

	//  로그인 요청 모델
	public class PlayerLoginRequest
	{
		public string PlayerName { get; set; }
		public string Password { get; set; }
	}
}
