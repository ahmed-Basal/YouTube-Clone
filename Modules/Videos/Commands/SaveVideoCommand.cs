using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Videos.Data;
using youtube.Modules.Videos.Entities;
using youtube.SharedKernel.Helpers;

namespace youtube.Modules.Videos.Commands
{
    public record SaveVideoCommand(
        int? Id,
        int ChannelId,
        string Title,
        string Description,
        int CategoryId,
        string VideoUrl,
        string ThumbnailUrl
    ) : IRequest<SaveVideoResult>;

    public class SaveVideoResult
    {
        public bool Success { get; set; }
        public bool NotFound { get; set; }
        public string Message { get; set; }
        public int VideoId { get; set; }
    }

    public class SaveVideoHandler : IRequestHandler<SaveVideoCommand, SaveVideoResult>
    {
        private readonly VideosDbContext _context;

        public SaveVideoHandler(VideosDbContext context)
        {
            _context = context;
        }

        public async Task<SaveVideoResult> Handle(SaveVideoCommand request, CancellationToken cancellationToken)
        {
            var videoUrl = request.VideoUrl;
            var thumbnailUrl = request.ThumbnailUrl;

            if (YouTubeHelper.IsYouTubeUrl(videoUrl))
            {
                videoUrl = YouTubeHelper.GetWatchUrl(videoUrl);
                if (string.IsNullOrEmpty(thumbnailUrl))
                {
                    thumbnailUrl = YouTubeHelper.GetThumbnailUrl(videoUrl);
                }
            }

            if (!request.Id.HasValue || request.Id.Value == 0)
            {
                var newVideo = new Video
                {
                    Title = request.Title?.Trim(),
                    Description = request.Description?.Trim() ?? string.Empty,
                    CategoryId = request.CategoryId,
                    ChannelId = request.ChannelId,
                    ThumbnailUrl = thumbnailUrl ?? string.Empty,
                    VideoUrl = videoUrl ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    Views = 0
                };

                _context.Videos.Add(newVideo);
                await _context.SaveChangesAsync(cancellationToken);

                return new SaveVideoResult
                {
                    Success = true,
                    VideoId = newVideo.Id,
                    Message = "Your video has been uploaded successfully!"
                };
            }
            else
            {
                var existingVideo = await _context.Videos
                    .FirstOrDefaultAsync(v => v.Id == request.Id.Value && v.ChannelId == request.ChannelId, cancellationToken);

                if (existingVideo == null)
                {
                    return new SaveVideoResult { NotFound = true, Message = "Video not found or you don't own it." };
                }

                existingVideo.Title = request.Title?.Trim();
                existingVideo.Description = request.Description?.Trim() ?? string.Empty;
                existingVideo.CategoryId = request.CategoryId;
                if (!string.IsNullOrEmpty(thumbnailUrl)) existingVideo.ThumbnailUrl = thumbnailUrl;
                if (!string.IsNullOrEmpty(videoUrl)) existingVideo.VideoUrl = videoUrl;

                await _context.SaveChangesAsync(cancellationToken);

                return new SaveVideoResult
                {
                    Success = true,
                    VideoId = existingVideo.Id,
                    Message = "Your video details have been updated!"
                };
            }
        }
    }
}
