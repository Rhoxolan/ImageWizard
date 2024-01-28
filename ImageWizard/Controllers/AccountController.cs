using ImageWizard.DTOs.AccountDTOs;
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

		[HttpPost("register")] //Идея для рефакторинга - подумать над тем, чтобы усложнить систему передачи пароля, мб как-то зашифровать передаваемый пароль
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(AccountDTO accountDTO)
		{
			var user = new IdentityUser{ UserName = accountDTO.Login };
			var createResult = await _userManager.CreateAsync(user, accountDTO.Password);
			if (!createResult.Succeeded)
			{
				return BadRequest(new { Errors = createResult.Errors });
			}
			throw new NotImplementedException();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(AccountDTO accountDTO)
		{
			throw new NotImplementedException();
		}
	}
}
