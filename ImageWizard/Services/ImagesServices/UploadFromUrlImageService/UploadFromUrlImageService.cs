using ImageWizard.DTOs.ImagesDTOs;

namespace ImageWizard.Services.ImagesServices.UploadFromUrlImageService
{
	public class UploadFromUrlImageService : IUploadFromUrlImageService
    {
		private readonly IHttpClientFactory _clientFactory;

		public UploadFromUrlImageService(IHttpClientFactory clientFactory)
        {
			_clientFactory = clientFactory;
		}

		public async Task<byte[]> GetImageBytesAsync(ImageDTO imageDTO)
		{
			var imageUri = new Uri(imageDTO.Url);
			var _httpClient = _clientFactory.CreateClient();
			byte[] imageBytes = await _httpClient.GetByteArrayAsync(imageUri);
			return imageBytes;
		}
	}
}
