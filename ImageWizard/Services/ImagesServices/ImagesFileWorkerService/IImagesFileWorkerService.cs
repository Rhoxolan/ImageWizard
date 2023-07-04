namespace ImageWizard.Services.ImagesServices.ImagesFileWorkerService
{
	public interface IImagesFileWorkerService
	{
		void SaveImage(string imageDirectory, string imagePath, Image image);

		void SaveImageThumbnail(string imagePath, Image image);

		void DeleteImage(string filepath, params string[] thumbnailPathes);
	}
}
