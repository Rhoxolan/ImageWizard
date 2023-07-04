using ImageWizard.Data.Entities;

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

		public void DeleteImage(string filepath, params string[] thumbnailPathes)
		{
			lock (_lock)
			{
				File.Delete(filepath);
				foreach(var thumbnailPath in thumbnailPathes)
				{
					if (File.Exists(thumbnailPath))
					{
						File.Delete(thumbnailPath);
					}
				}
			}
		}
	}
}
