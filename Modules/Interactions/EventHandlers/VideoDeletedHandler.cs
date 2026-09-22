using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Interactions.Data;
using youtube.SharedKernel.Events;

namespace youtube.Modules.Interactions.EventHandlers
{
    public class VideoDeletedHandler : INotificationHandler<VideoDeletedNotification>
    {
        private readonly InteractionsDbContext _context;

        public VideoDeletedHandler(InteractionsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(VideoDeletedNotification notification, CancellationToken cancellationToken)
        {
            var comments = await _context.Comments
                .Where(c => c.VideoId == notification.VideoId)
                .ToListAsync(cancellationToken);

            var reactions = await _context.LikesDislikes
                .Where(l => l.VideoId == notification.VideoId)
                .ToListAsync(cancellationToken);

            if (comments.Any())
            {
                _context.Comments.RemoveRange(comments);
            }

            if (reactions.Any())
            {
                _context.LikesDislikes.RemoveRange(reactions);
            }

            if (comments.Any() || reactions.Any())
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
