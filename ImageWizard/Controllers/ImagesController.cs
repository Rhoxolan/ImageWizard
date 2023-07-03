using ImageWizard.Data.Contexts;
using ImageWizard.DTOs.ImagesDTOs;
using ImageWizard.Filters.ImagesFilters;
using ImageWizard.Services.ImagesServices.GetImageUrlService;
using ImageWizard.Services.ImagesServices.SaveImageService;
using ImageWizard.Services.ImagesServices.UploadFromUrlImageService;
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
		private static readonly object _lock = new();
		private readonly IUploadFromUrlImageService _uploadFromUrlImageService;
		private readonly IImageService _imageService;
		private readonly IGetImageUrlService _getImageUrlService;

		public ImagesController(ImagesContext context, IUploadFromUrlImageService uploadFromUrlImageService,
			IImageService imageService, IGetImageUrlService getImageUrlService)
		{
			_context = context;
			_uploadFromUrlImageService = uploadFromUrlImageService;
			_imageService = imageService;
			_getImageUrlService = getImageUrlService;
		}

		[HttpPost]
		[WellFormedUriStringActionFilter]
		public async Task<IActionResult> PostImage(ImageDTO imageDTO)
		{
			try
			{
				var imageBytes = await _uploadFromUrlImageService.GetImageBytesAsync(imageDTO);
				if (imageBytes.Length > (5 * 1024 * 1024))
				{
					return UnprocessableEntity("The size of the image is bigger than 5MB");
				}
				int id = await _imageService.SaveImageAsync(imageBytes);
				return Ok(new { url = _getImageUrlService.GetImageUrl(id, Request.Scheme, Request.Host) });
			}
			catch (UnknownImageFormatException)
			{
				return UnprocessableEntity("The image format error");
			}
			catch
			{
				return Problem("Data processing error. Please contact to developer");
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetImage(int id)
		{
			try
			{
				var imageEntity = await _imageService.GetImageEntityAsync(id);
				if (imageEntity == null)
				{
					return NotFound();
				}
				return PhysicalFile(imageEntity.Path, _imageService.GetImageFormat(imageEntity.Path) ?? "img/*");
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
				var imageEntity = await _imageService.GetImageEntityAsync(id);
				if (imageEntity == null)
				{
					return NotFound();
				}
				string thumbnailFilePath = _imageService.GetImageThumbnailFilePath(imageEntity, size);
				return PhysicalFile(thumbnailFilePath, _imageService.GetImageFormat(thumbnailFilePath) ?? "img/*");
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
	}
}
