using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using youtube.Extentions;
using youtube.Features.Channal;
using youtube.viewmodels.account;

namespace youtube.Controllers
{
    [Authorize]
    public class ChannalController : CoreController
    {
        private readonly IMediator _mediator;

        public ChannalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(string stringModel)
        {
            var model = new ChannelAddEdit_vm();
            stringModel = HttpContext.Session.GetString("ChannelModelFromSession");

            if (!string.IsNullOrEmpty(stringModel))
            {
                model = JsonSerializer.Deserialize<ChannelAddEdit_vm>(stringModel);
                if (model != null && model.Errors.Count > 0)
                {
                    foreach (var error in model.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.ErrorMessage);
                    }

                    HttpContext.Session.Remove("ChannelModelFromSession");
                    return View(model);
                }
            }

            var userId = int.Parse(User.GetUserId());
            model = await _mediator.Send(new GetChannelQuery(userId));

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel(ChannelAddEdit_vm model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {
                        model.Errors.Add(new ModelError_vm
                        {
                            Key = item.Key,
                            ErrorMessage = item.Value.Errors.Select(x => x.ErrorMessage).FirstOrDefault()
                        });
                    }
                }

                HttpContext.Session.SetString("ChannelModelFromSession", JsonSerializer.Serialize(model));
                return RedirectToAction("Index");
            }

            var userId = int.Parse(User.GetUserId());
            var result = await _mediator.Send(new CreateChannelCommand(userId, model));

            if (!result.Success)
            {
                model.Errors.Add(new ModelError_vm
                {
                    Key = result.ErrorKey,
                    ErrorMessage = result.ErrorMessage
                });

                HttpContext.Session.SetString("ChannelModelFromSession", JsonSerializer.Serialize(model));
                return RedirectToAction("Index");
            }

            TempData["notification"] = "true;Channel Created;Your channel has been created and you can upload clips now";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditChannel(ChannelAddEdit_vm model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.GetUserId());
                var success = await _mediator.Send(new EditChannelCommand(userId, model));

                if (success)
                {
                    TempData["notification"] = "true;Channel updated;Your channel is updated";
                    return RedirectToAction("Index");
                }
            }

            TempData["notification"] = "false;Not Found;Your channel was not found";
            return RedirectToAction("Index");
        }
    }
}
