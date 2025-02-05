using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlayerController : ControllerBase
	{
		[HttpGet("{id}")]
		public IActionResult GetPlayerInfo(int id)
		{
			return Ok(new { id, name = "Player1", level = 10 });
		}
	}
}
