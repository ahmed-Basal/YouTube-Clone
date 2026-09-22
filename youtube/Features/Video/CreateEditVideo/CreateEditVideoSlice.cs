using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.Modules.Videos.Data;
using youtube.Modules.Videos.Entities;
using youtube.SharedKernel.Helpers;
using youtube.viewmodels.VideoVm;

namespace youtube.Features.Video.CreateEditVideo
{
    // ================= Query for GET Create/Edit =================
    public record GetVideoEditQuery(int Id, int UserId) : IRequest<GetVideoEditResult>;

    public class GetVideoEditResult
    {
        public bool ChannelNotFound { get; set; }
        public bool VideoNotFound { get; set; }
        public VideoEdit ViewModel { get; set; }
    }

    public class GetVideoEditHandler : IRequestHandler<GetVideoEditQuery, GetVideoEditResult>
    {
        private readonly ChannelsDbContext _channelsContext;
        private readonly VideosDbContext _videosContext;

        public GetVideoEditHandler(ChannelsDbContext channelsContext, VideosDbContext videosContext)
        {
            _channelsContext = channelsContext;
            _videosContext = videosContext;
        }

        public async Task<GetVideoEditResult> Handle(GetVideoEditQuery request, CancellationToken cancellationToken)
        {
            var channel = await _channelsContext.Channels
                .FirstOrDefaultAsync(x => x.AppUserId == request.UserId, cancellationToken);

            if (channel == null)
            {
                return new GetVideoEditResult { ChannelNotFound = true };
            }

            var categories = await _videosContext.Categories
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);

            var vm = new VideoEdit
            {
                CategoryDropdown = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            if (request.Id > 0)
            {
                var video = await _videosContext.Videos
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.ChannelId == channel.Id, cancellationToken);

                if (video == null)
                {
                    return new GetVideoEditResult { VideoNotFound = true };
                }

                vm.Id = video.Id;
                vm.Title = video.Title;
                vm.Description = video.Description;
                vm.CategoryId = video.CategoryId;
                vm.ImageUrl = video.ThumbnailUrl;
                vm.VideoUrl = video.VideoUrl;
                if (YouTubeHelper.IsYouTubeUrl(video.VideoUrl))
                {
                    vm.YouTubeUrl = video.VideoUrl;
                }
            }

            return new GetVideoEditResult { ViewModel = vm };
        }
    }

    // ================= Command for POST Create/Edit =================
    public record SaveVideoCommand(
        VideoEdit Model,
        int UserId,
        string CloudinaryThumbnailUrl = null,
        string CloudinaryVideoUrl = null
    ) : IRequest<SaveVideoResult>;

    public class SaveVideoResult
    {
        public bool Success { get; set; }
        public bool ChannelNotFound { get; set; }
        public bool VideoNotFound { get; set; }
        public string Message { get; set; }
    }

    public class SaveVideoHandler : IRequestHandler<SaveVideoCommand, SaveVideoResult>
    {
        private readonly ChannelsDbContext _channelsContext;
        private readonly VideosDbContext _videosContext;

        public SaveVideoHandler(ChannelsDbContext channelsContext, VideosDbContext videosContext)
        {
            _channelsContext = channelsContext;
            _videosContext = videosContext;
        }

        public async Task<SaveVideoResult> Handle(SaveVideoCommand request, CancellationToken cancellationToken)
        {
            var channel = await _channelsContext.Channels
                .FirstOrDefaultAsync(x => x.AppUserId == request.UserId, cancellationToken);

            if (channel == null)
            {
                return new SaveVideoResult { ChannelNotFound = true };
            }

            var model = request.Model;
            bool isYouTubeValid = !string.IsNullOrWhiteSpace(model.YouTubeUrl) && YouTubeHelper.IsYouTubeUrl(model.YouTubeUrl);

            string thumbnailUrl = !string.IsNullOrEmpty(request.CloudinaryThumbnailUrl) ? request.CloudinaryThumbnailUrl : model.ImageUrl;
            string videoUrl = !string.IsNullOrEmpty(request.CloudinaryVideoUrl) ? request.CloudinaryVideoUrl : model.VideoUrl;

            if (isYouTubeValid)
            {
                videoUrl = YouTubeHelper.GetWatchUrl(model.YouTubeUrl);
                if (string.IsNullOrEmpty(thumbnailUrl))
                {
                    thumbnailUrl = YouTubeHelper.GetThumbnailUrl(model.YouTubeUrl);
                }
            }

            if (model.Id == 0)
            {
                var newVideo = new youtube.Modules.Videos.Entities.Video
                {
                    Title = model.Title.Trim(),
                    Description = model.Description?.Trim() ?? string.Empty,
                    CategoryId = model.CategoryId,
                    ChannelId = channel.Id,
                    ThumbnailUrl = thumbnailUrl ?? string.Empty,
                    VideoUrl = videoUrl ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    Views = 0
                };

                _videosContext.Videos.Add(newVideo);
                await _videosContext.SaveChangesAsync(cancellationToken);
                return new SaveVideoResult { Success = true, Message = "Your video has been uploaded successfully!" };
            }
            else
            {
                var existingVideo = await _videosContext.Videos
                    .FirstOrDefaultAsync(v => v.Id == model.Id && v.ChannelId == channel.Id, cancellationToken);

                if (existingVideo == null)
                {
                    return new SaveVideoResult { VideoNotFound = true };
                }

                existingVideo.Title = model.Title.Trim();
                existingVideo.Description = model.Description?.Trim() ?? string.Empty;
                existingVideo.CategoryId = model.CategoryId;
                if (!string.IsNullOrEmpty(thumbnailUrl)) existingVideo.ThumbnailUrl = thumbnailUrl;
                if (!string.IsNullOrEmpty(videoUrl)) existingVideo.VideoUrl = videoUrl;

                await _videosContext.SaveChangesAsync(cancellationToken);
                return new SaveVideoResult { Success = true, Message = "Your video details have been updated!" };
            }
        }
    }
}
