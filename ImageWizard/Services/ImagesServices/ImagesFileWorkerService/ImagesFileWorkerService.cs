namespace ImageWizard.Services.ImagesServices.ImagesFileWorkerService
{
	public class ImagesFileWorkerService : IImagesFileWorkerService
	{
		private static readonly object _lock = new();

		public void SaveImage(string imageDirectory, string imagePath, Image image)
		{
			lock (_lock)
			{
				Directory.CreateDirectory(imageDirectory);
				image.Save(imagePath);
			}
		}

		public void SaveImageThumbnail(string imagePath, Image image)
		{
			lock (_lock)
			{
				image.Save(imagePath);
			}
		}
	}
}
