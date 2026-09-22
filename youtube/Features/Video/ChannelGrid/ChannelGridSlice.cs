using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.Modules.Interactions.Data;
using youtube.Modules.Videos.Data;

namespace youtube.Features.Video.ChannelGrid
{
    public record ChannelGridQuery(int UserId, int PageNumber = 1, int PageSize = 5, string SortBy = "") 
        : IRequest<ChannelGridResult>;

    public class ChannelGridResult
    {
        public IEnumerable<object> Items { get; set; } = new List<object>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItemsCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class ChannelGridHandler : IRequestHandler<ChannelGridQuery, ChannelGridResult>
    {
        private readonly ChannelsDbContext _channelsContext;
        private readonly VideosDbContext _videosContext;
        private readonly InteractionsDbContext _interactionsContext;

        public ChannelGridHandler(
            ChannelsDbContext channelsContext,
            VideosDbContext videosContext,
            InteractionsDbContext interactionsContext)
        {
            _channelsContext = channelsContext;
            _videosContext = videosContext;
            _interactionsContext = interactionsContext;
        }

        public async Task<ChannelGridResult> Handle(ChannelGridQuery request, CancellationToken cancellationToken)
        {
            var channel = await _channelsContext.Channels
                .FirstOrDefaultAsync(x => x.AppUserId == request.UserId, cancellationToken);

            if (channel == null)
            {
                return new ChannelGridResult
                {
                    PageNumber = 1,
                    PageSize = request.PageSize
                };
            }

            var query = _videosContext.Videos
                .Include(v => v.Category)
                .Where(v => v.ChannelId == channel.Id);

            query = request.SortBy switch
            {
                "title-a" => query.OrderBy(v => v.Title),
                "title-d" => query.OrderByDescending(v => v.Title),
                "date-a" => query.OrderBy(v => v.CreatedAt),
                "date-d" => query.OrderByDescending(v => v.CreatedAt),
                "views-a" => query.OrderBy(v => v.Views),
                "views-d" => query.OrderByDescending(v => v.Views),
                "category-a" => query.OrderBy(v => v.Category.Name),
                "category-d" => query.OrderByDescending(v => v.Category.Name),
                _ => query.OrderByDescending(v => v.CreatedAt)
            };

            var totalItems = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            var pagedVideos = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var videoIds = pagedVideos.Select(v => v.Id).ToList();

            var commentCounts = await _interactionsContext.Comments
                .Where(c => videoIds.Contains(c.VideoId))
                .GroupBy(c => c.VideoId)
                .Select(g => new { VideoId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.VideoId, x => x.Count, cancellationToken);

            var likeCounts = await _interactionsContext.LikesDislikes
                .Where(l => videoIds.Contains(l.VideoId) && l.like)
                .GroupBy(l => l.VideoId)
                .Select(g => new { VideoId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.VideoId, x => x.Count, cancellationToken);

            var dislikeCounts = await _interactionsContext.LikesDislikes
                .Where(l => videoIds.Contains(l.VideoId) && !l.like)
                .GroupBy(l => l.VideoId)
                .Select(g => new { VideoId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.VideoId, x => x.Count, cancellationToken);

            var items = pagedVideos.Select(v => (object)new
            {
                id = v.Id,
                thumbnailUrl = v.ThumbnailUrl,
                videoUrl = v.VideoUrl,
                title = v.Title,
                createdAt = v.CreatedAt,
                views = v.Views,
                comments = commentCounts.GetValueOrDefault(v.Id, 0),
                likes = likeCounts.GetValueOrDefault(v.Id, 0),
                dislikes = dislikeCounts.GetValueOrDefault(v.Id, 0),
                categoryName = v.Category != null ? v.Category.Name : ""
            }).ToList();

            return new ChannelGridResult
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItemsCount = totalItems,
                TotalPages = totalPages
            };
        }
    }
}
