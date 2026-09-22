using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Interactions.Contracts;
using youtube.Modules.Interactions.Data;

namespace youtube.Modules.Interactions.Queries
{
    public class InteractionQueryHandlers :
        IRequestHandler<GetVideoCommentsQuery, List<CommentDto>>,
        IRequestHandler<GetVideoReactionsQuery, VideoReactionsDto>
    {
        private readonly InteractionsDbContext _context;

        public InteractionQueryHandlers(InteractionsDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommentDto>> Handle(GetVideoCommentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.VideoId == request.VideoId)
                .OrderByDescending(c => c.PostAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    VideoId = c.VideoId,
                    AppUserId = c.AppUserId,
                    Content = c.Content,
                    PostAt = c.PostAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<VideoReactionsDto> Handle(GetVideoReactionsQuery request, CancellationToken cancellationToken)
        {
            var likesCount = await _context.LikesDislikes
                .CountAsync(l => l.VideoId == request.VideoId && l.like, cancellationToken);

            var dislikesCount = await _context.LikesDislikes
                .CountAsync(l => l.VideoId == request.VideoId && !l.like, cancellationToken);

            bool? currentUserLike = null;
            if (request.CurrentUserId.HasValue)
            {
                var reaction = await _context.LikesDislikes
                    .FirstOrDefaultAsync(l => l.VideoId == request.VideoId && l.AppUserId == request.CurrentUserId.Value, cancellationToken);

                if (reaction != null)
                {
                    currentUserLike = reaction.like;
                }
            }

            return new VideoReactionsDto
            {
                VideoId = request.VideoId,
                LikesCount = likesCount,
                DislikesCount = dislikesCount,
                CurrentUserLike = currentUserLike
            };
        }
    }
}
