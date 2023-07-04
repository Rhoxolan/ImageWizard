using ImageWizard.Data.Entities;
using ImageWizard.DTOs.ImagesDTOs;

namespace ImageWizard.Services.ImagesServices.SaveImageService
{
	public interface IImageService
	{
		Task<int> SaveImageAsync(byte[] imageBytes);

		Task<LocalImageDTO?> GetLocalImageAsync(int id);

		Task<LocalImageDTO?> GetLocalImageThumbnailAsync(int id, int size);
	}
}
