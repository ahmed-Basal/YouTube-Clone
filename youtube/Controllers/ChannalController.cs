using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UTlity;

namespace youtube.Controllers
{
    public class ChannalController : Controller
    {
        [Authorize(Roles = $"{SD.UserRole}")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
