using ImageWizard.Data.Contexts;
using ImageWizard.Data.Entities;
using ImageWizard.DTOs.ImagesDTOs;
using ImageWizard.Filters.ImagesFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;

namespace ImageWizard.Controllers
{
	[Route("api/images")]
	[ApiController]
	public class ImagesController : ControllerBase
	{
		private readonly ImagesContext _context;
		private readonly IHttpClientFactory _clientFactory;
		private static readonly object _lock = new();

		public ImagesController(ImagesContext context, IHttpClientFactory clientFactory)
		{
			_context = context;
			_clientFactory = clientFactory;
		}

		[HttpPost]
		[WellFormedUriStringActionFilter]
		public async Task<IActionResult> PostImage(ImageDTO imageDTO)
		{
			try
			{
				var imageUri = new Uri(imageDTO.Url);
				var _httpClient = _clientFactory.CreateClient();
				byte[] imageBytes = await _httpClient.GetByteArrayAsync(imageUri);
				if (imageBytes.Length > (5 * 1024 * 1024))
				{
					//Return code 422 because the request is valid but the
					//server cannot process it because the image is too large
					return UnprocessableEntity("The size of the image is bigger than 5MB");
				}
				var format = Image.DetectFormat(imageBytes);
				using Image image = Image.Load(imageBytes);
				string imageDirectory = GetNewImageDirectoryName();
				string imageName = $"{Guid.NewGuid()}.{format.Name.ToLower()}";
				string imagePath = Path.Combine(imageDirectory, imageName);
				lock (_lock)
				{
					Directory.CreateDirectory(imageDirectory);
					image.Save(imagePath);
				}
				ImageEntity imageEntity = new ImageEntity
				{
					Path = imagePath
				};
				_context.ImageEntities.Add(imageEntity);
				await _context.SaveChangesAsync();
				return Ok(new { url = GetImageUrl(imageEntity.Id) });
			}
			catch (UnknownImageFormatException)
			{
				//Return code 422 because the request is valid but the content type is not valid
				return UnprocessableEntity("The image format error");
			}
			catch
			{
				//Return code 400 because an error occurred while processing the data.
				return BadRequest("Data processing error. Please contact to developer");
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetImage(int id)
		{
			try
			{
				var imageEntity = await _context.ImageEntities.FindAsync(id);
				if (imageEntity == null)
				{
					return NotFound();
				}
				using Image image = Image.Load(imageEntity.Path);
				return PhysicalFile(imageEntity.Path, image.Metadata.DecodedImageFormat?.DefaultMimeType ?? "img/*");
			}
			catch (InvalidImageContentException)
			{
				return Problem("The image has problems. Please contact to developer");
			}
			catch (UnknownImageFormatException)
			{
				return Problem("Problems with the image format. Please contact to developer");
			}
			catch
			{
				return Problem("Data processing error. Please contact to developer");
			}
		}

		[HttpGet("{id}/size/{size:regex(^(100|300)$)}")]
		public async Task<IActionResult> GetImageThumbnail(int id, int size)
		{
			try
			{
				var imageEntity = await _context.ImageEntities.FindAsync(id);
				if (imageEntity == null)
				{
					return NotFound();
				}
				if (!System.IO.File.Exists(imageEntity.Path))
				{
					return Problem("Data processing error. Please contact to developer");
				}
				string folderPath = Path.GetDirectoryName(imageEntity.Path)!;
				string mainFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageEntity.Path)!;
				string mainFileNameExtension = Path.GetExtension(imageEntity.Path)!.ToLower();
				string thumbnailFilePath = Path.Combine(folderPath, $"{mainFileNameWithoutExtension}-{size}{mainFileNameExtension}");
				if (!System.IO.File.Exists(thumbnailFilePath))
				{
					using Image image = Image.Load(imageEntity.Path);
					using Image imageThumbnail = image.Clone(i => i.Resize(size, size));
					lock (_lock)
					{
						imageThumbnail.Save(thumbnailFilePath);
					}
					return PhysicalFile(thumbnailFilePath, imageThumbnail.Metadata.DecodedImageFormat?.DefaultMimeType ?? "img/*");
				}
				using Image imageThumbnailed = Image.Load(thumbnailFilePath);
				return PhysicalFile(thumbnailFilePath, imageThumbnailed.Metadata.DecodedImageFormat?.DefaultMimeType ?? "img/*");
			}
			catch (InvalidImageContentException)
			{
				return Problem("The image has problems. Please contact to developer");
			}
			catch (UnknownImageFormatException)
			{
				return Problem("Problems with the image format. Please contact to developer");
			}
			catch
			{
				return Problem("Data processing error. Please contact to developer");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteImage(int id)
		{
			var imageEntity = await _context.ImageEntities.FindAsync(id);
			if (imageEntity == null)
			{
				return NotFound();
			}
			string folderPath = Path.GetDirectoryName(imageEntity.Path)!;
			string mainFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageEntity.Path)!;
			string mainFileNameExtension = Path.GetExtension(imageEntity.Path)!.ToLower();
			string thumbnail100FilePath = Path.Combine(folderPath, $"{mainFileNameWithoutExtension}-100{mainFileNameExtension}");
			string thumbnail300FilePath = Path.Combine(folderPath, $"{mainFileNameWithoutExtension}-300{mainFileNameExtension}");
			if (!System.IO.File.Exists(imageEntity.Path))
			{
				return Problem("Data processing error. Please contact to developer");
			}
			lock (_lock)
			{
				System.IO.File.Delete(imageEntity.Path);
				if (System.IO.File.Exists(thumbnail100FilePath))
				{
					System.IO.File.Delete(thumbnail100FilePath);
				}
				if (System.IO.File.Exists(thumbnail300FilePath))
				{
					System.IO.File.Delete(thumbnail300FilePath);
				}
			}
			_context.ImageEntities.Remove(imageEntity);
			await _context.SaveChangesAsync();
			return Ok();
		}

		private string GetThumbnailImageUrl(int id, int size)
		{
			return $"{Request.Scheme}://{Request.Host}/api/images/get-url/{id}/size/{size}";
		}

		private string GetImageUrl(int id)
		{
			return $"{Request.Scheme}://{Request.Host}/api/images/get-url/{id}";
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
