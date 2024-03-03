using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;

namespace ImageWizard.Middlewares
{
	public class ImagesProcessingAccessMiddleware
	{
		private readonly RequestDelegate _next;

		public ImagesProcessingAccessMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			bool isPostImages = context.Request.Path.Value == "/api/images" && context.Request.Method.ToUpper() == "POST";
			bool isGetImages = context.Request.Path.Value!.StartsWith("/api/images/") && context.Request.Method.ToUpper() == "GET";
			bool isContainsAuthHeader = !string.IsNullOrEmpty(context.Request.Headers["Authorization"]);

			if (isContainsAuthHeader && (isPostImages || isGetImages))
			{
				var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
				if (!authResult.Succeeded)
				{
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
					context.Response.Headers.AccessControlAllowOrigin = "*";
					context.Response.Headers.ContentType = "application/problem+json; charset=utf-8";
					return;
				}
			}

			await _next(context);
		}
	}
}
