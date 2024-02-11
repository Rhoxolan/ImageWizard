using ImageWizard.Data.Entities;
using ImageWizard.DTOs.ImagesDTOs;

namespace ImageWizard.Services.ImagesServices.SaveImageService
{
	public interface IImageService
	{
		Task<ImageEntity> SaveImageAsync(byte[] imageBytes, User? user);

		Task<LocalImageDTO?> GetLocalImageAsync(int id, User? user);

		Task<LocalImageDTO?> GetLocalImageThumbnailAsync(int id, int size, User user);

		Task<ImageEntity?> GetImageEntityByUserAsync(int id, User user);

		Task<ImageEntity?> GetImageEntityAsync(int id);

		Task DeleteImageAsync(ImageEntity imageEntity);
	}
}
