using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using youtube.Modules.Channels.Data;

namespace youtube.Modules.Channels
{
    public static class ChannelsModuleExtensions
    {
        public static IServiceCollection AddChannelsModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ChannelsDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(ChannelsModuleExtensions).Assembly));

            return services;
        }
    }
}
