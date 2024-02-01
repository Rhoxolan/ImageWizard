using ImageWizard.DTOs.ImagesDTOs;
using ImageWizard.Services.ImagesServices.UploadFromUrlImageService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ImageWizard.Filters.ImagesFilters
{
	public class ValidImageSizeActionFilterAttribute : ActionFilterAttribute
	{
		private readonly IUploadFromUrlImageService _uploadFromUrlImageService;

		public ValidImageSizeActionFilterAttribute(IUploadFromUrlImageService uploadFromUrlImageService)
		{
			_uploadFromUrlImageService = uploadFromUrlImageService;
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var imageDTO = (ImageDTO)context.ActionArguments["imageDTO"]!;
			var imageBytes = await _uploadFromUrlImageService.GetImageBytesAsync(imageDTO);
			if (imageBytes.Length > (5 * 1024 * 1024))
			{
				context.Result = new UnprocessableEntityObjectResult("The size of the image is bigger than 5MB");
			}
			else
			{
				context.HttpContext.Items["imageBytes"] = imageBytes;
			}
			await next();
		}
	}
}
