using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImageWizard.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<IdentityUser> _userManager;

		public AccountController(UserManager<IdentityUser> userManager)
		{
			_userManager = userManager;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register()
		{
			throw new NotImplementedException();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login()
		{
			throw new NotImplementedException();
		}
	}
}
