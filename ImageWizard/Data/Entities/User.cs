using Microsoft.AspNetCore.Identity;

namespace ImageWizard.Data.Entities
{
	public class User : IdentityUser
	{
		public List<ImageEntity> Images { get; set; } = default!;
	}
}
