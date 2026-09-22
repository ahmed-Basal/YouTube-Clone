using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.Modules.Videos.Data;
using youtube.Modules.Videos.Entities;
using youtube.viewmodels;

namespace youtube.Features.Category
{
    public record GetCategoryDetailsQuery(int Id) : IRequest<CategoryDetailsResult>;

    public class CategoryDetailsResult
    {
        public bool NotFound { get; set; }
        public youtube.Modules.Videos.Entities.Category Category { get; set; }
        public IEnumerable<VideoCard_vm> Videos { get; set; }
    }

    public class GetCategoryDetailsHandler : IRequestHandler<GetCategoryDetailsQuery, CategoryDetailsResult>
    {
        private readonly VideosDbContext _videosContext;
        private readonly ChannelsDbContext _channelsContext;

        public GetCategoryDetailsHandler(VideosDbContext videosContext, ChannelsDbContext channelsContext)
        {
            _videosContext = videosContext;
            _channelsContext = channelsContext;
        }

        public async Task<CategoryDetailsResult> Handle(GetCategoryDetailsQuery request, CancellationToken cancellationToken)
        {
            var category = await _videosContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category == null)
            {
                return new CategoryDetailsResult { NotFound = true };
            }

            var videos = await _videosContext.Videos
                .AsNoTracking()
                .Where(v => v.CategoryId == request.Id)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync(cancellationToken);

            var channelIds = videos.Select(v => v.ChannelId).Distinct().ToList();
            var channels = await _channelsContext.Channels
                .AsNoTracking()
                .Where(c => channelIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, cancellationToken);

            var videoVms = videos.Select(v => new VideoCard_vm
            {
                Id = v.Id,
                Title = v.Title,
                Description = v.Description,
                ThumbnailUrl = v.ThumbnailUrl,
                VideoUrl = v.VideoUrl,
                Views = v.Views,
                CreatedAt = v.CreatedAt,
                CategoryId = v.CategoryId,
                ChannelId = v.ChannelId,
                Channal = channels.TryGetValue(v.ChannelId, out var ch) ? new VideoCardChannel
                {
                    Id = ch.Id,
                    Name = ch.Name
                } : new VideoCardChannel { Id = v.ChannelId, Name = "Creator" },
                Category = new VideoCardCategory
                {
                    Id = category.Id,
                    Name = category.Name
                }
            }).ToList();

            return new CategoryDetailsResult
            {
                Category = category,
                Videos = videoVms
            };
        }
    }
}
