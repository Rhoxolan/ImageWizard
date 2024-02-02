using ImageWizard.DTOs.ImagesDTOs;
using ImageWizard.Filters.ImagesFilters;
using ImageWizard.Services.ImagesServices.GetImageUrlService;
using ImageWizard.Services.ImagesServices.SaveImageService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImageWizard.Controllers
{
	[Route("api/images")]
	[ApiController]
	public class ImagesController : ControllerBase
	{
		private readonly IImageService _imageService;
		private readonly IGetImageUrlService _getImageUrlService;
		private readonly UserManager<IdentityUser> _userManager;

		public ImagesController(IImageService imageService, IGetImageUrlService getImageUrlService, UserManager<IdentityUser> userManager)
		{
			_imageService = imageService;
			_getImageUrlService = getImageUrlService;
			_userManager = userManager;
		}

		[HttpPost]
		[WellFormedUriStringActionFilter(Order = int.MinValue)]
		[ServiceFilter(typeof(ValidImageSizeActionFilterAttribute))]
		public async Task<IActionResult> PostImage(ImageDTO imageDTO)
		{
			try
			{
				var imageBytes = (byte[])HttpContext.Items["imageBytes"]!;
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
				var localImage = await _imageService.GetLocalImageAsync(id);
				if (localImage == null)
				{
					return NotFound();
				}
				return PhysicalFile(localImage.Path, localImage.Format ?? "img/*");
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
				var localImageThumbnail = await _imageService.GetLocalImageThumbnailAsync(id, size);
				if (localImageThumbnail == null)
				{
					return NotFound();
				}
				return PhysicalFile(localImageThumbnail.Path, localImageThumbnail.Format ?? "img/*");
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
			try
			{
				var imageEntity = await _imageService.GetImageEntityAsync(id);
				if (imageEntity == null)
				{
					return NotFound();
				}
				await _imageService.DeleteImageAsync(imageEntity);
				return Ok();
			}
			catch
			{
				return Problem("Data processing error. Please contact to developer");
			}
		}
	}
}
