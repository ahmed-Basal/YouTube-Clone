using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using youtube.Modules.Users.Data;
using youtube.Modules.Users.Entities;

namespace youtube.Modules.Users
{
    public static class UsersModuleExtensions
    {
        public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<UsersDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<UsersDbContext>()
            .AddDefaultTokenProviders();

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(UsersModuleExtensions).Assembly));

            return services;
        }
    }
}
