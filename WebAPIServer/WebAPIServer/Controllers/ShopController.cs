using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebAPIServer.Models;

namespace WebAPIServer.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ShopController : ControllerBase
	{
		private readonly DataBaseContext _context;

		public ShopController(DataBaseContext context)
		{
			_context = context;
		}

		// GET: /api/shop
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Character>>> GetAvailableCharacters()
		{
			return await _context.Characters.Where(c => c.IsOnSale).ToListAsync();
		}

		// POST: /api/shop/buy
		[HttpPost("buy")]
		[Authorize] // JWT 필요
		public async Task<ActionResult> BuyCharacter([FromBody] int characterId)
		{
			var playerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			var alreadyOwned = await _context.UserCharacters
				.AnyAsync(uc => uc.PlayerId == playerId && uc.CharacterId == characterId);

			if (alreadyOwned)
				return BadRequest(new { message = "이미 구매한 캐릭터입니다." });

			var character = await _context.Characters.FindAsync(characterId);
			if (character == null || !character.IsOnSale)
				return NotFound(new { message = "존재하지 않거나 판매 중이 아닌 캐릭터입니다." });

			// 구매 처리 (코인 차감 등은 생략됨. 필요 시 Player에 Coins 컬럼 추가)
			var userCharacter = new UserCharacter
			{
				PlayerId = playerId,
				CharacterId = characterId
			};

			_context.UserCharacters.Add(userCharacter);
			await _context.SaveChangesAsync();

			return Ok(new { message = "캐릭터 구매 완료!" });
		}

		// GET: /api/shop/my-characters
		[HttpGet("my-characters")]
		[Authorize]
		public async Task<ActionResult> GetMyCharacters()
		{
			var playerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			var myCharacters = await _context.UserCharacters
				.Include(uc => uc.Character)
				.Where(uc => uc.PlayerId == playerId)
				.Select(uc => new
				{
					uc.CharacterId,
					uc.Character.Name,
					uc.Character.Description,
					uc.AcquiredAt
				}).ToListAsync();

			return Ok(myCharacters);
		}
	}

}
