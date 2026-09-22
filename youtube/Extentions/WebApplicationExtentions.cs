using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MediatR;
using youtube.Modules.Users;
using youtube.Modules.Channels;
using youtube.Modules.Videos;
using youtube.Modules.Interactions;
using youtube.Modules.Administration;
using youtube.SharedKernel;

namespace youtube.Extentions
{
    public static class WebApplicationExtentions
    {
        public static WebApplicationBuilder Applicationbuilder(this WebApplicationBuilder builder)
        {
            // Register Modular Monolith Modules
            builder.Services.AddUsersModule(builder.Configuration);
            builder.Services.AddChannelsModule(builder.Configuration);
            builder.Services.AddVideosModule(builder.Configuration);
            builder.Services.AddInteractionsModule(builder.Configuration);
            builder.Services.AddAdministrationModule(builder.Configuration);

            builder.Services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(typeof(WebApplicationExtentions).Assembly));

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            // 100 MB max upload request body size configuration
            builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
            });
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = 104857600; // 100 MB
            });

            return builder;
        }
    }
}