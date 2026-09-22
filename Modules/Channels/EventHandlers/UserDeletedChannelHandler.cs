using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.SharedKernel.Events;

namespace youtube.Modules.Channels.EventHandlers
{
    public class UserDeletedChannelHandler : INotificationHandler<UserDeletedNotification>
    {
        private readonly ChannelsDbContext _context;

        public UserDeletedChannelHandler(ChannelsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
        {
            var userSubs = await _context.Subscriptions
                .Where(s => s.AppUserId == notification.UserId)
                .ToListAsync(cancellationToken);

            if (userSubs.Any())
            {
                _context.Subscriptions.RemoveRange(userSubs);
            }

            var channel = await _context.Channels
                .Include(c => c.Subscriptions)
                .FirstOrDefaultAsync(c => c.AppUserId == notification.UserId, cancellationToken);

            if (channel != null)
            {
                if (channel.Subscriptions.Any())
                {
                    _context.Subscriptions.RemoveRange(channel.Subscriptions);
                }

                _context.Channels.Remove(channel);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
