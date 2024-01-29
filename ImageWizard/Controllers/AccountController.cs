using ImageWizard.DTOs.AccountDTOs;
using ImageWizard.Services.JWTService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImageWizard.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IJWTService _JWTService;

		public AccountController(UserManager<IdentityUser> userManager, IJWTService JWTService)
		{
			_userManager = userManager;
			_JWTService = JWTService;
		}

		[HttpPost("register")] //Идея для рефакторинга - подумать над тем, чтобы усложнить систему передачи пароля, мб как-то зашифровать передаваемый пароль
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(AccountDTO accountDTO)
		{
			var user = new IdentityUser
			{
				UserName = accountDTO.Login
			};
			var createResult = await _userManager.CreateAsync(user, accountDTO.Password);
			if (!createResult.Succeeded)
			{
				return BadRequest(new { Errors = createResult.Errors });
			}
			return Ok(new { token = _JWTService.GenerateJWTToken(user) });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(AccountDTO accountDTO)
		{
			throw new NotImplementedException();
		}
	}
}
