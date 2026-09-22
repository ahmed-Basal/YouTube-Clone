using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.Modules.Videos.Contracts;
using youtube.Modules.Videos.Data;
using youtube.viewmodels;

namespace youtube.Features.Home.GetFeed
{
    public record GetFeedQuery(string Search = null, int? CategoryId = null) : IRequest<HomeFeed_vm>;

    public class GetFeedHandler : IRequestHandler<GetFeedQuery, HomeFeed_vm>
    {
        private readonly VideosDbContext _videosContext;
        private readonly ChannelsDbContext _channelsContext;

        public GetFeedHandler(VideosDbContext videosContext, ChannelsDbContext channelsContext)
        {
            _videosContext = videosContext;
            _channelsContext = channelsContext;
        }

        public async Task<HomeFeed_vm> Handle(GetFeedQuery request, CancellationToken cancellationToken)
        {
            var categories = await _videosContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);

            var query = _videosContext.Videos
                .AsNoTracking()
                .Include(v => v.Category)
                .AsQueryable();

            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(v => v.CategoryId == request.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim().ToLower();
                query = query.Where(v => v.Title.ToLower().Contains(term) || v.Description.ToLower().Contains(term));
            }

            var videosList = await query
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync(cancellationToken);

            var channelIds = videosList.Select(v => v.ChannelId).Distinct().ToList();
            var channels = await _channelsContext.Channels
                .AsNoTracking()
                .Where(c => channelIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, cancellationToken);

            var videoVms = videosList.Select(v => new VideoCard_vm
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
                Category = v.Category != null ? new VideoCardCategory
                {
                    Id = v.Category.Id,
                    Name = v.Category.Name
                } : null
            }).ToList();

            return new HomeFeed_vm
            {
                Videos = videoVms,
                Categories = categories,
                ActiveCategoryId = request.CategoryId,
                SearchQuery = request.Search
            };
        }
    }
}
