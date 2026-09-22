using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using youtube.Features.Category;

namespace youtube.Controllers
{
    public class CategoryController : CoreController
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _mediator.Send(new GetCategoryDetailsQuery(id));
            if (result.NotFound)
            {
                return NotFound();
            }

            ViewBag.CategoryName = result.Category.Name;
            ViewBag.CategoryId = result.Category.Id;

            return View(result.Videos);
        }
    }
}
