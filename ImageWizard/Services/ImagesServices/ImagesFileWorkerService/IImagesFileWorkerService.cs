namespace ImageWizard.Services.ImagesServices.ImagesFileWorkerService
{
	public interface IImagesFileWorkerService
	{
		public void SaveImage(string imageDirectory, string imagePath, Image image);
	}
}
