using Microsoft.EntityFrameworkCore;

namespace ImageWizard.Services.ImagesServices.SaveImageService
{
	public interface ISaveImageService
	{
		Task<int> SaveImageAsync(byte[] imageBytes);
	}
}
