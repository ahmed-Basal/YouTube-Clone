using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using youtube.Extentions;
using youtube.Features.Video.ChannelGrid;
using youtube.Features.Video.CreateEditVideo;
using youtube.Features.Video.WatchVideo;
using youtube.Modules.Channels.Contracts;
using youtube.Modules.Channels.Commands;
using youtube.Modules.Interactions.Commands;
using youtube.Modules.Users.Contracts;
using youtube.Modules.Videos.Commands;
using youtube.Modules.Videos.Services;
using youtube.SharedKernel;
using youtube.SharedKernel.Helpers;
using youtube.viewmodels;
using youtube.viewmodels.VideoVm;

namespace youtube.Controllers
{
    public class VideoController : CoreController
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public VideoController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        // ================= GET /Video/CreateEditVideo/{id?} =================
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreateEditVideo(int id)
        {
            var userId = int.Parse(User.GetUserId());
            var result = await _mediator.Send(new GetVideoEditQuery(id, userId));

            if (result.ChannelNotFound)
            {
                TempData["notification"] = "false;Channel Required;You need to create a channel first before uploading videos.";
                return RedirectToAction("Index", "Channal");
            }

            if (result.VideoNotFound)
            {
                return NotFound();
            }

            return View(result.ViewModel);
        }

        // ================= POST /Video/CreateEditVideo =================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)] // 100 MB Limit
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)] // 100 MB Limit
        public async Task<IActionResult> CreateEditVideo(VideoEdit model, [FromServices] ICloudinaryService cloudinaryService)
        {
            var userId = int.Parse(User.GetUserId());

            bool hasYouTubeUrl = !string.IsNullOrWhiteSpace(model.YouTubeUrl);
            bool isYouTubeValid = hasYouTubeUrl && YouTubeHelper.IsYouTubeUrl(model.YouTubeUrl);

            if (hasYouTubeUrl && !isYouTubeValid)
            {
                ModelState.AddModelError("YouTubeUrl", "Please enter a valid YouTube URL (e.g. https://www.youtube.com/watch?v=... or https://youtu.be/...)");
            }

            if (model.Id == 0)
            {
                bool hasVideoUpload = model.VideoUpload != null && model.VideoUpload.Length > 0;
                if (!hasVideoUpload && !isYouTubeValid)
                {
                    ModelState.AddModelError("VideoUpload", "Please upload a video file or provide a valid YouTube link.");
                }
            }

            if (!ModelState.IsValid)
            {
                var queryResult = await _mediator.Send(new GetVideoEditQuery(model.Id, userId));
                if (queryResult.ViewModel != null)
                {
                    model.CategoryDropdown = queryResult.ViewModel.CategoryDropdown;
                }
                return View(model);
            }

            var allowedImageTypes = _configuration.GetSection("FileUpload:imageContentTypes").Get<string[]>() 
                ?? new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };
            var allowedVideoTypes = _configuration.GetSection("FileUpload:videoContentTypes").Get<string[]>() 
                ?? new[] { "video/mp4", "video/webm", "video/ogg", "video/avi", "video/quicktime" };

            string uploadedThumbnailUrl = null;
            string uploadedVideoUrl = null;

            if (model.ImageUpload != null && model.ImageUpload.Length > 0)
            {
                if (!allowedImageTypes.Contains(model.ImageUpload.ContentType.ToLower()))
                {
                    ModelState.AddModelError("ImageUpload", "Invalid image format. Supported: JPG, PNG, WebP, GIF.");
                    var queryResult = await _mediator.Send(new GetVideoEditQuery(model.Id, userId));
                    if (queryResult.ViewModel != null) model.CategoryDropdown = queryResult.ViewModel.CategoryDropdown;
                    return View(model);
                }

                uploadedThumbnailUrl = await cloudinaryService.UploadImageAsync(model.ImageUpload, "vidpulse_thumbnails");
            }

            if (model.VideoUpload != null && model.VideoUpload.Length > 0)
            {
                if (!allowedVideoTypes.Contains(model.VideoUpload.ContentType.ToLower()))
                {
                    ModelState.AddModelError("VideoUpload", "Invalid video format. Supported: MP4, WebM, Ogg.");
                    var queryResult = await _mediator.Send(new GetVideoEditQuery(model.Id, userId));
                    if (queryResult.ViewModel != null) model.CategoryDropdown = queryResult.ViewModel.CategoryDropdown;
                    return View(model);
                }

                uploadedVideoUrl = await cloudinaryService.UploadVideoAsync(model.VideoUpload, "vidpulse_videos");
            }

            var saveResult = await _mediator.Send(new youtube.Features.Video.CreateEditVideo.SaveVideoCommand(model, userId, uploadedThumbnailUrl, uploadedVideoUrl));

            if (saveResult.ChannelNotFound)
            {
                TempData["notification"] = "false;Channel Required;You need to create a channel first.";
                return RedirectToAction("Index", "Channal");
            }

            if (saveResult.VideoNotFound)
            {
                return NotFound();
            }

            TempData["notification"] = $"true;{(model.Id == 0 ? "Video Uploaded" : "Video Updated")};{saveResult.Message}";
            return RedirectToAction("Index", "Channal");
        }

        // ================= GET /Video/GetVideosForChannelGrid =================
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetVideosForChannelGrid(int pageNumber = 1, int pageSize = 5, string sortBy = "")
        {
            var userId = int.Parse(User.GetUserId());
            var result = await _mediator.Send(new ChannelGridQuery(userId, pageNumber, pageSize, sortBy));

            return Json(new ApiResponse(200, result: new
            {
                items = result.Items,
                pageNumber = result.PageNumber,
                pageSize = result.PageSize,
                totalItemsCount = result.TotalItemsCount,
                totalPages = result.TotalPages
            }));
        }

        // ================= DELETE /Video/DeleteVideo/{id} =================
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var userId = int.Parse(User.GetUserId());
            var channelResult = await _mediator.Send(new GetChannelByUserIdQuery(userId));
            if (!channelResult.IsSuccess)
            {
                return Json(new ApiResponse(403, "Forbidden", "You do not own a channel"));
            }

            var result = await _mediator.Send(new youtube.Modules.Videos.Commands.DeleteVideoCommand(id, channelResult.Value.Id));
            return Json(new ApiResponse(result.StatusCode, result.Title, result.Message));
        }

        // ================= GET /Video/Watch/{id} =================
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Watch(int id)
        {
            int? currentUserId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                currentUserId = int.Parse(User.GetUserId());
            }

            var vm = await _mediator.Send(new WatchVideoQuery(id, currentUserId));
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        // ================= POST /Video/ToggleLike =================
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleLike(int videoId, bool isLike)
        {
            var userId = int.Parse(User.GetUserId());
            var result = await _mediator.Send(new youtube.Modules.Interactions.Commands.ToggleLikeCommand(videoId, userId, isLike));
            return Json(result);
        }

        // ================= POST /Video/ToggleSubscribe =================
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleSubscribe(int channelId)
        {
            var userId = int.Parse(User.GetUserId());
            var result = await _mediator.Send(new youtube.Modules.Channels.Commands.ToggleSubscribeCommand(channelId, userId));
            return Json(result);
        }

        // ================= POST /Video/AddComment =================
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int videoId, string text)
        {
            var userId = int.Parse(User.GetUserId());
            var result = await _mediator.Send(new youtube.Modules.Interactions.Commands.AddCommentCommand(videoId, userId, text));
            if (!result.IsSuccess)
            {
                return Json(new { success = false, message = result.Error });
            }

            var userSummary = await _mediator.Send(new youtube.Modules.Users.Contracts.GetUserSummaryQuery(userId));
            var comment = result.Value;

            return Json(new
            {
                success = true,
                commentData = new
                {
                    id = comment.Id,
                    text = comment.Content,
                    createdAt = comment.PostAt.ToString("MMM dd, yyyy"),
                    userName = userSummary.IsSuccess ? (userSummary.Value.Name ?? userSummary.Value.UserName) : "User",
                    userAvatar = "/images/default-avatar.png"
                }
            });
        }

        // ================= POST /Video/DeleteComment =================
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = int.Parse(User.GetUserId());
            var success = await _mediator.Send(new youtube.Modules.Interactions.Commands.DeleteCommentCommand(commentId, userId));
            return Json(new { success });
        }
    }
}
