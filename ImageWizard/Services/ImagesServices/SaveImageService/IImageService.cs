using ImageWizard.Data.Entities;
using ImageWizard.DTOs.ImagesDTOs;

namespace ImageWizard.Services.ImagesServices.SaveImageService
{
	public interface IImageService
	{
		Task<ImageEntity> SaveImageAsync(byte[] imageBytes);

		Task<ImageEntity> SaveImageWithUserAsync(byte[] imageBytes, User user);

		Task<LocalImageDTO?> GetLocalImageAsync(int id);

		Task<LocalImageDTO?> GetLocalImageByUserIdAsync(int id, string userId);

		Task<LocalImageDTO?> GetLocalImageThumbnailAsync(int id, int size, string userId);

		Task<ImageEntity?> GetImageEntityByUserIdAsync(int id, string userId);

		Task<ImageEntity?> GetImageEntityAsync(int id);

		Task DeleteImageAsync(ImageEntity imageEntity);
	}
}
