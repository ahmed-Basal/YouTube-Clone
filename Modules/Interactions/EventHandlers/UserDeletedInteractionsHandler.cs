using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Interactions.Data;
using youtube.SharedKernel.Events;

namespace youtube.Modules.Interactions.EventHandlers
{
    public class UserDeletedInteractionsHandler : INotificationHandler<UserDeletedNotification>
    {
        private readonly InteractionsDbContext _context;

        public UserDeletedInteractionsHandler(InteractionsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
        {
            var comments = await _context.Comments
                .Where(c => c.AppUserId == notification.UserId)
                .ToListAsync(cancellationToken);

            var likes = await _context.LikesDislikes
                .Where(l => l.AppUserId == notification.UserId)
                .ToListAsync(cancellationToken);

            if (comments.Any())
            {
                _context.Comments.RemoveRange(comments);
            }

            if (likes.Any())
            {
                _context.LikesDislikes.RemoveRange(likes);
            }

            if (comments.Any() || likes.Any())
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
