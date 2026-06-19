using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using youtube.DataAccess.Data;

namespace youtube.Extentions
{
    public static class WebApplicationExtentions
    {
       public static  WebApplicationBuilder Applicationbuilder(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<Context>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("youtube.DataAccess")));
             return builder;
        }
       
    }
}