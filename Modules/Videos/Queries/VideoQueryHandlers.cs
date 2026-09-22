using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Videos.Contracts;
using youtube.Modules.Videos.Data;
using youtube.SharedKernel;

namespace youtube.Modules.Videos.Queries
{
    public record GetHomeVideosQuery(int? CategoryId = null, string SearchTerm = null) : IRequest<List<VideoSummaryDto>>;
    public record GetRelatedVideosQuery(int VideoId, int Limit = 8) : IRequest<List<VideoSummaryDto>>;

    public class VideoQueryHandlers :
        IRequestHandler<GetVideoSummaryQuery, Result<VideoSummaryDto>>,
        IRequestHandler<GetVideosByChannelQuery, List<VideoSummaryDto>>,
        IRequestHandler<GetCategoriesQuery, List<CategoryDto>>,
        IRequestHandler<GetHomeVideosQuery, List<VideoSummaryDto>>,
        IRequestHandler<GetRelatedVideosQuery, List<VideoSummaryDto>>
    {
        private readonly VideosDbContext _context;

        public VideoQueryHandlers(VideosDbContext context)
        {
            _context = context;
        }

        public async Task<Result<VideoSummaryDto>> Handle(GetVideoSummaryQuery request, CancellationToken cancellationToken)
        {
            var video = await _context.Videos
                .AsNoTracking()
                .Include(v => v.Category)
                .Where(v => v.Id == request.VideoId)
                .Select(v => new VideoSummaryDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    Description = v.Description,
                    ThumbnailUrl = v.ThumbnailUrl,
                    VideoUrl = v.VideoUrl,
                    Views = v.Views,
                    CategoryId = v.CategoryId,
                    CategoryName = v.Category != null ? v.Category.Name : string.Empty,
                    ChannelId = v.ChannelId,
                    CreatedAt = v.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (video == null)
            {
                return Result<VideoSummaryDto>.Failure("Video not found", 404);
            }

            return Result<VideoSummaryDto>.Success(video);
        }

        public async Task<List<VideoSummaryDto>> Handle(GetVideosByChannelQuery request, CancellationToken cancellationToken)
        {
            return await _context.Videos
                .AsNoTracking()
                .Include(v => v.Category)
                .Where(v => v.ChannelId == request.ChannelId)
                .OrderByDescending(v => v.CreatedAt)
                .Select(v => new VideoSummaryDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    Description = v.Description,
                    ThumbnailUrl = v.ThumbnailUrl,
                    VideoUrl = v.VideoUrl,
                    Views = v.Views,
                    CategoryId = v.CategoryId,
                    CategoryName = v.Category != null ? v.Category.Name : string.Empty,
                    ChannelId = v.ChannelId,
                    CreatedAt = v.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<VideoSummaryDto>> Handle(GetHomeVideosQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Videos
                .AsNoTracking()
                .Include(v => v.Category)
                .AsQueryable();

            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(v => v.CategoryId == request.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(v => v.Title.ToLower().Contains(term) || v.Description.ToLower().Contains(term));
            }

            return await query
                .OrderByDescending(v => v.CreatedAt)
                .Select(v => new VideoSummaryDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    Description = v.Description,
                    ThumbnailUrl = v.ThumbnailUrl,
                    VideoUrl = v.VideoUrl,
                    Views = v.Views,
                    CategoryId = v.CategoryId,
                    CategoryName = v.Category != null ? v.Category.Name : string.Empty,
                    ChannelId = v.ChannelId,
                    CreatedAt = v.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<VideoSummaryDto>> Handle(GetRelatedVideosQuery request, CancellationToken cancellationToken)
        {
            return await _context.Videos
                .AsNoTracking()
                .Include(v => v.Category)
                .Where(v => v.Id != request.VideoId)
                .OrderByDescending(v => v.CreatedAt)
                .Take(request.Limit)
                .Select(v => new VideoSummaryDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    Description = v.Description,
                    ThumbnailUrl = v.ThumbnailUrl,
                    VideoUrl = v.VideoUrl,
                    Views = v.Views,
                    CategoryId = v.CategoryId,
                    CategoryName = v.Category != null ? v.Category.Name : string.Empty,
                    ChannelId = v.ChannelId,
                    CreatedAt = v.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
