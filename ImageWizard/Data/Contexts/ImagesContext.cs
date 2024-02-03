using ImageWizard.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ImageWizard.Data.Contexts
{
	public class ImagesContext : IdentityDbContext<User>
	{
		public ImagesContext(DbContextOptions<ImagesContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		public DbSet<ImageEntity> ImageEntities { get; set; }
	}
}
