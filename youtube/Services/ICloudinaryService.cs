using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace youtube.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "vidpulse_thumbnails");
        Task<string> UploadVideoAsync(IFormFile file, string folder = "vidpulse_videos");
    }
}
