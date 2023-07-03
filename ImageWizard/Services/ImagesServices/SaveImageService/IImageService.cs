using ImageWizard.Data.Entities;

namespace ImageWizard.Services.ImagesServices.SaveImageService
{
	public interface IImageService
	{
		Task<int> SaveImageAsync(byte[] imageBytes);

		Task<ImageEntity?> GetImageEntityAsync(int id);

		string GetImageThumbnailFilePath(ImageEntity imageEntity, int size);

		string? GetImageFormat(string path);
	}
}
