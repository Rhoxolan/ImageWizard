﻿using ImageWizard.Data.Entities;
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
		private readonly UserManager<User> _userManager;
		private readonly IJWTService _JWTService;

		public AccountController(UserManager<User> userManager, IJWTService JWTService)
		{
			_userManager = userManager;
			_JWTService = JWTService;
		}

		[HttpPost("register")] //Идея для рефакторинга - подумать над тем, чтобы усложнить систему передачи пароля, мб как-то зашифровать передаваемый пароль
		public async Task<IActionResult> Register(AccountDTO accountDTO)
		{
			try
			{
				var user = new User
				{
					UserName = accountDTO.Login
				};
				var createResult = await _userManager.CreateAsync(user, accountDTO.Password);
				if (!createResult.Succeeded)
				{
					return BadRequest(new { createResult.Errors });
				}
				var token = _JWTService.GenerateJWTToken(user);
				return Ok(new { token });
			}
			catch
			{
				return Problem("Error. Please contact to developer");
			}
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(AccountDTO accountDTO)
		{
			try
			{
				var user = await _userManager.FindByNameAsync(accountDTO.Login);
				if (user == null || !await _userManager.CheckPasswordAsync(user, accountDTO.Password))
				{
					return Unauthorized();
				}
				var token = _JWTService.GenerateJWTToken(user);
				return Ok(new { token });
			}
			catch
			{
				return Problem("Error. Please contact to developer");
			}
		}
	}
}
