using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace youtube.Modules.Videos.Services
{
    public interface IHashingService
    {
        Task<string> ComputeFileHashAsync(IFormFile file);
    }

    public class HashingService : IHashingService
    {
        public async Task<string> ComputeFileHashAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Guid.NewGuid().ToString("N");
            }

            using var sha256 = SHA256.Create();
            await using var stream = file.OpenReadStream();
            var hashBytes = await sha256.ComputeHashAsync(stream);

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
