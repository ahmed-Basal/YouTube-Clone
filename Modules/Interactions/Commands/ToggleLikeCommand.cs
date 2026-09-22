using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Interactions.Data;
using youtube.Modules.Interactions.Entities;

namespace youtube.Modules.Interactions.Commands
{
    public record ToggleLikeCommand(int VideoId, int UserId, bool IsLike) : IRequest<ToggleLikeResult>;

    public class ToggleLikeResult
    {
        public bool Success { get; set; }
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
    }

    public class ToggleLikeHandler : IRequestHandler<ToggleLikeCommand, ToggleLikeResult>
    {
        private readonly InteractionsDbContext _context;

        public ToggleLikeHandler(InteractionsDbContext context)
        {
            _context = context;
        }

        public async Task<ToggleLikeResult> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
        {
            var reaction = await _context.LikesDislikes
                .FirstOrDefaultAsync(l => l.VideoId == request.VideoId && l.AppUserId == request.UserId, cancellationToken);

            bool userLiked = false;
            bool userDisliked = false;

            if (reaction != null)
            {
                if (reaction.like == request.IsLike)
                {
                    _context.LikesDislikes.Remove(reaction);
                }
                else
                {
                    reaction.like = request.IsLike;
                    userLiked = request.IsLike;
                    userDisliked = !request.IsLike;
                }
            }
            else
            {
                _context.LikesDislikes.Add(new LikeDislike
                {
                    AppUserId = request.UserId,
                    VideoId = request.VideoId,
                    like = request.IsLike
                });
                userLiked = request.IsLike;
                userDisliked = !request.IsLike;
            }

            await _context.SaveChangesAsync(cancellationToken);

            var likesCount = await _context.LikesDislikes.CountAsync(l => l.VideoId == request.VideoId && l.like, cancellationToken);
            var dislikesCount = await _context.LikesDislikes.CountAsync(l => l.VideoId == request.VideoId && !l.like, cancellationToken);

            return new ToggleLikeResult
            {
                Success = true,
                IsLiked = userLiked,
                IsDisliked = userDisliked,
                LikesCount = likesCount,
                DislikesCount = dislikesCount
            };
        }
    }
}
