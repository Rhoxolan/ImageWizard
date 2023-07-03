namespace ImageWizard.Services.ImagesServices.GetImageUrlService
{
	public interface IGetImageUrlService
	{
		string GetImageUrl(int id, string scheme, HostString host);
	}
}
