using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPIServer.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


namespace WebAPIServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")] // API 경로: /api/player
	public class PlayerController : ControllerBase
	{
		private readonly DataBaseContext _context;
		private readonly JwtService _jwtService;

		public PlayerController(DataBaseContext context, JwtService jwtService)
		{
			_context = context;
			_jwtService = jwtService;
		}


		// 모든 플레이어 조회 (GET /api/player)
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
		{
			return await _context.Players.ToListAsync();
		}

		// 특정 플레이어 조회 (GET /api/player/{id})
		[Authorize(Roles = "Player")] // JWT 인증된 사용자만 접근 가능
		[HttpGet("{id}")]
		public async Task<ActionResult<Player>> GetPlayer(int id, [FromHeader] string authorization)
		{
			// 기본적인 인증 
			var playerIdFromToken = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "0");

			//  추가적인 검증 (JWT를 수동으로 확인)
			var token = authorization.Substring("Bearer ".Length).Trim();
			var claimsPrincipal = _jwtService.ValidateToken(token);

			if (claimsPrincipal == null)
			{
				return Unauthorized(new { message = "유효하지 않은 JWT 토큰입니다." });
			}

			//   사용자가 자신의 정보만 조회할 수 있도록 제한
			if (id != playerIdFromToken)
			{
				return Forbid(); 
			}

			var player = await _context.Players.FindAsync(id);
			if (player == null)
			{
				return NotFound(new { message = "해당 ID의 플레이어를 찾을 수 없습니다." });
			}

			return player;
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
			// JWT 토큰 생성
			var token = _jwtService.GenerateToken(player.PlayerId, player.PlayerName);

			//  할일 JWT 발급
			 return Ok(new
            {
                message = "로그인 성공!",
                token,
                playerId = player.PlayerId
            });
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
