using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using youtube.core.IRepo;

namespace youtube.Controllers
{
    public class CoreController : Controller
    {
        private IUnirOFWork _unitOfWork;
        protected IUnirOFWork UnitOfWork => _unitOfWork ??= HttpContext.RequestServices.GetService<IUnirOFWork>();
    }
}
