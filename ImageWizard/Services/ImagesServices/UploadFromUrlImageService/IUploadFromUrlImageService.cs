using ImageWizard.DTOs.ImagesDTOs;

namespace ImageWizard.Services.ImagesServices.UploadFromUrlImageService
{
	public interface IUploadFromUrlImageService
    {
		Task<byte[]> GetImageBytesAsync(ImageDTO imageDTO);
	}
}
