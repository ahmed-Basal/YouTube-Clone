using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace youtube.Modules.Administration
{
    public static class AdministrationModuleExtensions
    {
        public static IServiceCollection AddAdministrationModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(AdministrationModuleExtensions).Assembly));

            return services;
        }
    }
}
