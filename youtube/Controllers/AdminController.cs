using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UTlity;
using youtube.core.Entities;
using youtube.viewmodels;

namespace youtube.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class AdminController : CoreController
    {
        [HttpGet]
        public IActionResult Category()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await UnitOfWork.Category.GetAllAsync();
            return Json(new ApiResponse(200, "Success", "Categories loaded successfully", categories));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(new ApiResponse(400, "Error", "Category name is required"));
            }

            var category = new Category { Name = name.Trim() };
            UnitOfWork.Category.Add(category);
            await UnitOfWork.CompleteAsync();

            return Json(new ApiResponse(200, "Success", "Category created successfully", category));
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, string name)
        {
            var category = await UnitOfWork.Category.GetByIdAsync(id);
            if (category == null)
            {
                return Json(new ApiResponse(404, "Error", "Category not found"));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(new ApiResponse(400, "Error", "Category name is required"));
            }

            var destination = new Category { Id = id, Name = name.Trim() };
            UnitOfWork.Category.Update(category, destination);
            await UnitOfWork.CompleteAsync();

            return Json(new ApiResponse(200, "Success", "Category updated successfully", destination));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await UnitOfWork.Category.GetByIdAsync(id);
            if (category == null)
            {
                return Json(new ApiResponse(404, "Error", "Category not found"));
            }

            UnitOfWork.Category.Remove(category);
            await UnitOfWork.CompleteAsync();

            return Json(new ApiResponse(200, "Success", "Category deleted successfully"));
        }
    }
}
