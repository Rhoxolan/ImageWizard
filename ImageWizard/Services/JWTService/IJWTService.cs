using ImageWizard.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace ImageWizard.Services.JWTService
{
	public interface IJWTService
	{
		string GenerateJWTToken(User user);
	}
}
