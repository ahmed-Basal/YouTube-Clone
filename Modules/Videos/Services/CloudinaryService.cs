using System;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace youtube.Modules.Videos.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "vidpulse_thumbnails");
        Task<string> UploadVideoAsync(IFormFile file, string folder = "vidpulse_videos");
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IHashingService _hashingService;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IOptions<CloudinarySettings> config, IHashingService hashingService, ILogger<CloudinaryService> logger)
        {
            _hashingService = hashingService;
            _logger = logger;

            if (config != null && !string.IsNullOrEmpty(config.Value?.CloudName))
            {
                var acc = new Account(
                    config.Value.CloudName,
                    config.Value.ApiKey,
                    config.Value.ApiSecret
                );
                _cloudinary = new Cloudinary(acc);
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "vidpulse_thumbnails")
        {
            if (file == null || file.Length == 0) return null;

            var fileHash = await _hashingService.ComputeFileHashAsync(file);

            try
            {
                if (_cloudinary != null)
                {
                    await using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = $"{folder}/{fileHash}",
                        Overwrite = true
                    };

                    var result = await _cloudinary.UploadAsync(uploadParams);
                    if (result?.SecureUrl != null)
                    {
                        return result.SecureUrl.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cloudinary image upload failed. Falling back to local disk.");
            }

            return await SaveLocallyAsync(file, "thumbnails", fileHash);
        }

        public async Task<string> UploadVideoAsync(IFormFile file, string folder = "vidpulse_videos")
        {
            if (file == null || file.Length == 0) return null;

            var fileHash = await _hashingService.ComputeFileHashAsync(file);

            try
            {
                if (_cloudinary != null)
                {
                    await using var stream = file.OpenReadStream();
                    var uploadParams = new VideoUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = $"{folder}/{fileHash}",
                        Overwrite = true
                    };

                    var result = await _cloudinary.UploadLargeAsync(uploadParams);
                    if (result?.SecureUrl != null)
                    {
                        return result.SecureUrl.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cloudinary video upload failed. Falling back to local disk.");
            }

            return await SaveLocallyAsync(file, "videos", fileHash);
        }

        private async Task<string> SaveLocallyAsync(IFormFile file, string subFolder, string hash)
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", subFolder);
            if (!Directory.Exists(uploadsDir))
            {
                uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "youtube", "wwwroot", "uploads", subFolder);
            }
            Directory.CreateDirectory(uploadsDir);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{hash}{ext}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{subFolder}/{fileName}";
        }
    }
}
