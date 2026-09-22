using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace youtube.Controllers
{
    public class CoreController : Controller
    {
        private IConfiguration _configuration;
        protected IConfiguration Configuration => _configuration ??= HttpContext.RequestServices.GetService<IConfiguration>();
    }
}
