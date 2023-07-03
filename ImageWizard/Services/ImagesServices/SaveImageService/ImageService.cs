using ImageWizard.Data.Contexts;
using ImageWizard.Data.Entities;
using ImageWizard.Services.ImagesServices.ImagesFileWorkerService;

namespace ImageWizard.Services.ImagesServices.SaveImageService
{
	public class ImageService : IImageService
	{
		private readonly IImagesFileWorkerService _imagesFileWorkerService;
		private readonly ImagesContext _context;

		public ImageService(IImagesFileWorkerService imagesFileWorkerService, ImagesContext context)
		{
			_imagesFileWorkerService = imagesFileWorkerService;
			_context = context;
		}

		public async Task<int> SaveImageAsync(byte[] imageBytes)
		{
			var format = Image.DetectFormat(imageBytes);
			using Image image = Image.Load(imageBytes);
			string imageDirectory = GetNewImageDirectoryName();
			string imageName = $"{Guid.NewGuid()}.{format.Name.ToLower()}";
			string imagePath = Path.Combine(imageDirectory, imageName);
			_imagesFileWorkerService.SaveImage(imageDirectory, imagePath, image);
			ImageEntity imageEntity = new ImageEntity
			{
				Path = imagePath
			};
			_context.ImageEntities.Add(imageEntity);
			await _context.SaveChangesAsync();
			return imageEntity.Id;
		}

		public async Task<ImageEntity?> GetImageEntityAsync(int id)
		{
			return await _context.ImageEntities.FindAsync(id);
		}

		public string? GetImageFormat(string path)
		{
			using Image image = Image.Load(path);
			return image.Metadata.DecodedImageFormat?.DefaultMimeType;
		}

		private string GetNewImageDirectoryName()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "Images",
				$"{GetRandomLetter()}{GetRandomLetter()}", $"{GetRandomLetter()}{GetRandomLetter()}");
		}

		private string GetRandomLetter()
		{
			const string letters = "qwertyuiopasdfghjklzxcvbnm";
			var rand = new Random();
			return letters[rand.Next(letters.Length)].ToString();
		}
	}
}
