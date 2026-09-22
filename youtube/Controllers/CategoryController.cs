using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using youtube.core.Entities;

namespace youtube.Controllers
{
    public class CategoryController : CoreController
    {
        public async Task<IActionResult> Details(int id)
        {
            var category = await UnitOfWork.Category.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Retrieve videos associated with this category, including their channel info
            var videos = await UnitOfWork.Video.GetAllAsync(
                v => v.CategoryId == id, 
                includeProperties: "Channal"
            );

            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = category.Id;

            return View(videos);
        }
    }
}
