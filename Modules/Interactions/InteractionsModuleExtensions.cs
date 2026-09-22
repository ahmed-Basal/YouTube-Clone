using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using youtube.Modules.Interactions.Data;

namespace youtube.Modules.Interactions
{
    public static class InteractionsModuleExtensions
    {
        public static IServiceCollection AddInteractionsModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<InteractionsDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(InteractionsModuleExtensions).Assembly));

            return services;
        }
    }
}
