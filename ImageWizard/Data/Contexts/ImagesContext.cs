using ImageWizard.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImageWizard.Data.Contexts
{
	public class ImagesContext : DbContext
	{
		public ImagesContext(DbContextOptions<ImagesContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		public DbSet<ImageEntity> ImageEntities { get; set; }
	}
}
