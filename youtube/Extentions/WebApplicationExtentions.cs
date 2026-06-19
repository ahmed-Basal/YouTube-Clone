using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using youtube.DataAccess.Data;
using youtube.core.Entities;

namespace youtube.Extentions
{
    public static class WebApplicationExtentions
    {
       public static  WebApplicationBuilder Applicationbuilder(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<Context>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("youtube.DataAccess")));

            builder.Services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();

             return builder;
        }

    }
}