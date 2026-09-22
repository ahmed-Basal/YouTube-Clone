using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace youtube.Services
{
    public interface IHashingService
    {
        Task<string> ComputeFileHashAsync(IFormFile file);
    }
}
