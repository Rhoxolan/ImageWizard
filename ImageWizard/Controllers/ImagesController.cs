using ImageWizard.Data.Entities;
using ImageWizard.DTOs.ImagesDTOs;
using ImageWizard.Filters.ImagesFilters;
using ImageWizard.Services.ImagesServices.GetImageUrlService;
using ImageWizard.Services.ImagesServices.SaveImageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageWizard.Controllers
{
	[Route("api/images")]
	[ApiController]
	public class ImagesController : ControllerBase
	{
		private readonly IImageService _imageService;
		private readonly IGetImageUrlService _getImageUrlService;
		private readonly UserManager<User> _userManager;

		public ImagesController(IImageService imageService, IGetImageUrlService getImageUrlService, UserManager<User> userManager)
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
				ImageEntity image;
				User? currentUser = null;
				var imageBytes = (byte[])HttpContext.Items["imageBytes"]!;
				if (User.Identity != null && User.Identity.IsAuthenticated)
				{
					var userId = User.FindFirst(ClaimTypes.NameIdentifier);
					if (userId != null)
					{
						currentUser = await _userManager.FindByIdAsync(userId.Value);
					}
				}
				if (currentUser != null)
				{
					image = await _imageService.SaveImageWithUserAsync(imageBytes, currentUser);
				}
				else
				{
					image = await _imageService.SaveImageAsync(imageBytes);
				}
				return Ok(new { url = _getImageUrlService.GetImageUrl(image.Id, Request.Scheme, Request.Host) });
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
				LocalImageDTO? localImage = null;
				Claim? userId = null;
				var image = await _imageService.GetImageEntityAsync(id);
				if (image == null)
				{
					return NotFound();
				}
				if(image.User != null)
				{
					if (User.Identity == null || !User.Identity.IsAuthenticated)
					{
						return NotFound();
					}
					userId = User.FindFirst(ClaimTypes.NameIdentifier)!;
					localImage = await _imageService.GetLocalImageByUserIdAsync(id, userId.Value);
				}
				else
				{
					localImage = await _imageService.GetLocalImageAsync(id);
				}
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
		[Authorize]
		public async Task<IActionResult> GetImageThumbnail(int id, int size)
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)!;
				var localImageThumbnail = await _imageService.GetLocalImageThumbnailAsync(id, size, userId.Value);
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
		[Authorize]
		public async Task<IActionResult> DeleteImage(int id)
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)!;
				var imageEntity = await _imageService.GetImageEntityByUserIdAsync(id, userId.Value);
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
