using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using youtube.Modules.Videos.Data;
using youtube.Modules.Videos.Services;

namespace youtube.Modules.Videos
{
    public static class VideosModuleExtensions
    {
        public static IServiceCollection AddVideosModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<VideosDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IHashingService, HashingService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(VideosModuleExtensions).Assembly));

            return services;
        }
    }
}
