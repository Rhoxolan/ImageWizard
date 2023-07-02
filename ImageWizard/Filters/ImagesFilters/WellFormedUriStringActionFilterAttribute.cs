using ImageWizard.DTOs.ImagesDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ImageWizard.Filters.ImagesFilters
{
	public class WellFormedUriStringActionFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (context.ActionArguments.TryGetValue("imageDTO", out var imageDTO))
			{
				if (!Uri.IsWellFormedUriString(((ImageDTO)imageDTO!).Url, UriKind.Absolute))
				{
					context.Result = new BadRequestObjectResult("Wrong URI");
				}
			}
		}
	}
}
