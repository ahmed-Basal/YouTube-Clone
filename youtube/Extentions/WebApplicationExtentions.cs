using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using youtube.core.Entities;
using youtube.core.IRepo;
using youtube.DataAccess.Data;
using youtube.DataAccess.Repo;

namespace youtube.Extentions
{
    public static class WebApplicationExtentions
    {
        public static WebApplicationBuilder Applicationbuilder(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("youtube.DataAccess")));
            builder.Services.AddScoped<IUnirOFWork, UnitOfWork>();

            builder.Services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            return builder;
        }
    }
}