using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UTlity;
using youtube.Extentions;
using youtube.viewmodels.account;
using youtube.core.Entities;

// Updated: Corrected typos like 'ChanncelAddEdit_vm' and 'JsonConvert'
namespace youtube.Controllers
{
    [Authorize(Roles = $"{SD.UserRole}")]
    public class ChannalController : CoreController
    {
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
            var channel = await UnitOfWork.Channal.GetFirstOrDefaultAsync(x => x.AppUserId == userId, includeProperties: "subScriptions");

            if (channel != null)
            {
                model.Name = channel.Name;
                model.About = channel.About;
                model.SubscribersCount = channel.subScriptions?.Count ?? 0;
            }

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

            var channelNameExists = await UnitOfWork.Channal.AnyAsync(x => x.Name.ToLower() == model.Name.ToLower());
            if (channelNameExists)
            {
                model.Errors.Add(new ModelError_vm
                {
                    Key = "Name",
                    ErrorMessage = $"Channel name of {model.Name} is taken. Please try other name"
                });

                HttpContext.Session.SetString("ChannelModelFromSession", JsonSerializer.Serialize(model));
                return RedirectToAction("Index");
            }

            var channelToAdd = new Channal
            {
                AppUserId = int.Parse(User.GetUserId()),
                Name = model.Name,
                About = model.About,
            };

            UnitOfWork.Channal.Add(channelToAdd);
            await UnitOfWork.CompleteAsync();

            TempData["notification"] = "true;Channel Created;Your channel has been created and you can upload clips now";

            return RedirectToAction("Index");
        }
    }
}
