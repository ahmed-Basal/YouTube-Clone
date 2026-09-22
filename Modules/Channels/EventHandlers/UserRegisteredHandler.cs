using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.Modules.Channels.Entities;
using youtube.SharedKernel.Events;

namespace youtube.Modules.Channels.EventHandlers
{
    public class UserRegisteredHandler : INotificationHandler<UserRegisteredNotification>
    {
        private readonly ChannelsDbContext _context;

        public UserRegisteredHandler(ChannelsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
        {
            var exists = await _context.Channels
                .AnyAsync(c => c.AppUserId == notification.UserId, cancellationToken);

            if (exists)
            {
                return;
            }

            var channelName = !string.IsNullOrWhiteSpace(notification.Name)
                ? $"{notification.Name}'s Channel"
                : $"{notification.UserName}'s Channel";

            var channel = new Channel
            {
                AppUserId = notification.UserId,
                Name = channelName,
                About = $"Welcome to {channelName}!",
                CreatedAt = DateTime.UtcNow
            };

            _context.Channels.Add(channel);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
