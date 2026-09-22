using System.Diagnostics;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using youtube.Features.Home.GetFeed;
using youtube.Models;
using youtube.viewmodels;

namespace youtube.Controllers
{
    public class HomeController : CoreController
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(string search = null, int? categoryId = null)
        {
            var vm = await _mediator.Send(new GetFeedQuery(search, categoryId));
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
