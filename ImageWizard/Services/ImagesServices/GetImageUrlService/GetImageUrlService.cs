namespace ImageWizard.Services.ImagesServices.GetImageUrlService
{
	public class GetImageUrlService : IGetImageUrlService
	{
		public string GetImageUrl(int id, string scheme, HostString host)
		{
			return $"{scheme}://{host}/api/images/get-url/{id}";
		}
	}
}
