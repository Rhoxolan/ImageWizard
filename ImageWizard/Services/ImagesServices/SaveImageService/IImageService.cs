using ImageWizard.Data.Entities;
using ImageWizard.DTOs.ImagesDTOs;
using Microsoft.EntityFrameworkCore;

namespace ImageWizard.Services.ImagesServices.SaveImageService
{
	public interface IImageService
	{
		Task<int> SaveImageAsync(byte[] imageBytes);

		Task<LocalImageDTO?> GetLocalImageAsync(int id);

		Task<LocalImageDTO?> GetLocalImageThumbnailAsync(int id, int size);

		Task<ImageEntity?> GetImageEntityAsync(int id);

		DbSet<ImageEntity> GetImageEntities();

		Task DeleteImageAsync(ImageEntity imageEntity);
	}
}
