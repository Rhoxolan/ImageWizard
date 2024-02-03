using ImageWizard.Data.Contexts;
using ImageWizard.Data.Entities;
using ImageWizard.DTOs.ImagesDTOs;
using ImageWizard.Services.ImagesServices.ImagesFileWorkerService;
using Microsoft.EntityFrameworkCore;

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

		public async Task<LocalImageDTO?> GetLocalImageAsync(int id)
		{
			var imageEntity = await _context.ImageEntities.FindAsync(id);
			if (imageEntity == null)
			{
				return null;
			}
			using Image image = Image.Load(imageEntity.Path);
			return new LocalImageDTO
			{
				Path = imageEntity.Path,
				Format = image.Metadata.DecodedImageFormat?.DefaultMimeType
			};
		}

		public async Task<LocalImageDTO?> GetLocalImageThumbnailAsync(int id, int size)
		{
			var imageEntity = await _context.ImageEntities.FindAsync(id);
			if (imageEntity == null)
			{
				return null;
			}
			string folderPath = Path.GetDirectoryName(imageEntity.Path)!;
			string mainFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageEntity.Path)!;
			string mainFileNameExtension = Path.GetExtension(imageEntity.Path)!.ToLower();
			string thumbnailFilePath = Path.Combine(folderPath, $"{mainFileNameWithoutExtension}-{size}{mainFileNameExtension}");
			if (!File.Exists(thumbnailFilePath))
			{
				using Image image = Image.Load(imageEntity.Path);
				using Image imageThumbnail = image.Clone(i => i.Resize(size, size));
				_imagesFileWorkerService.SaveImageThumbnail(thumbnailFilePath, imageThumbnail);
				return new LocalImageDTO
				{
					Path = thumbnailFilePath,
					Format = imageThumbnail.Metadata.DecodedImageFormat?.DefaultMimeType
				};
			}
			using Image localImageThumbnail = Image.Load(thumbnailFilePath);
			return new LocalImageDTO
			{
				Path = thumbnailFilePath,
				Format = localImageThumbnail.Metadata.DecodedImageFormat?.DefaultMimeType
			};
		}

		public async Task DeleteImageAsync(ImageEntity imageEntity)
		{
			string folderPath = Path.GetDirectoryName(imageEntity.Path)!;
			string mainFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageEntity.Path)!;
			string mainFileNameExtension = Path.GetExtension(imageEntity.Path)!.ToLower();
			string thumbnail100FilePath = Path.Combine(folderPath, $"{mainFileNameWithoutExtension}-100{mainFileNameExtension}");
			string thumbnail300FilePath = Path.Combine(folderPath, $"{mainFileNameWithoutExtension}-300{mainFileNameExtension}");
			_imagesFileWorkerService.DeleteImage(imageEntity.Path, thumbnail100FilePath, thumbnail300FilePath);
			_context.ImageEntities.Remove(imageEntity);
			await _context.SaveChangesAsync();
		}

		public async Task<ImageEntity?> GetImageEntityAsync(int id)
		{
			return await _context.ImageEntities.FindAsync(id);
		}

		public DbSet<ImageEntity> GetImageEntities()
		{
			return _context.ImageEntities;
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
