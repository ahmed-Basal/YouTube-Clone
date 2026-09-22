using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using youtube.Modules.Administration.Commands;
using youtube.Modules.Administration.Queries;
using youtube.SharedKernel;
using youtube.viewmodels;

namespace youtube.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class AdminController : CoreController
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var currentUserId = 0;
            var currentUserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(currentUserIdClaim, out currentUserId);

            var dtos = await _mediator.Send(new GetAdminUsersQuery(currentUserId));
            var userVms = dtos.Select(d => new UserManagement_vm
            {
                Id = d.Id,
                Name = d.Name,
                UserName = d.UserName,
                Email = d.Email,
                CreatedAt = d.CreatedAt,
                Roles = d.Roles,
                ChannelName = d.ChannelName,
                ChannelId = d.ChannelId,
                VideoCount = d.VideoCount,
                IsCurrentAdmin = d.IsCurrentAdmin
            }).ToList();

            return View(userVms);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(currentUserIdClaim, out int currentUserId);

            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                         Request.ContentType?.Contains("application/json") == true;

            var result = await _mediator.Send(new DeleteAdminUserCommand(id, currentUserId));

            if (!result.Success)
            {
                if (isAjax) return Json(new { success = false, message = result.Message });
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Users));
            }

            if (isAjax) return Json(new { success = true, message = result.Message });
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public IActionResult Category()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _mediator.Send(new GetAdminCategoriesQuery());
            return Json(new ApiResponse(200, "Success", "Categories loaded successfully", categories));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(string name)
        {
            var result = await _mediator.Send(new CreateAdminCategoryCommand(name));
            if (!result.Success)
            {
                return Json(new ApiResponse(400, "Error", result.Message));
            }

            return Json(new ApiResponse(200, "Success", result.Message, result.Category));
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, string name)
        {
            var result = await _mediator.Send(new EditAdminCategoryCommand(id, name));
            if (!result.Success)
            {
                return Json(new ApiResponse(result.Message.Contains("not found") ? 404 : 400, "Error", result.Message));
            }

            return Json(new ApiResponse(200, "Success", result.Message, result.Category));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _mediator.Send(new DeleteAdminCategoryCommand(id));
            if (!result.Success)
            {
                return Json(new ApiResponse(404, "Error", result.Message));
            }

            return Json(new ApiResponse(200, "Success", result.Message));
        }
    }
}
