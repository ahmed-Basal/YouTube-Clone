using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.Modules.Channels.Entities;

namespace youtube.Modules.Channels.Commands
{
    public record ToggleSubscribeCommand(int ChannelId, int UserId) : IRequest<ToggleSubscribeResult>;

    public class ToggleSubscribeResult
    {
        public bool Success { get; set; }
        public bool IsSubscribed { get; set; }
        public int SubscribersCount { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ToggleSubscribeHandler : IRequestHandler<ToggleSubscribeCommand, ToggleSubscribeResult>
    {
        private readonly ChannelsDbContext _context;

        public ToggleSubscribeHandler(ChannelsDbContext context)
        {
            _context = context;
        }

        public async Task<ToggleSubscribeResult> Handle(ToggleSubscribeCommand request, CancellationToken cancellationToken)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.ChannalId == request.ChannelId && s.AppUserId == request.UserId, cancellationToken);

            bool isSubscribed;
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                isSubscribed = false;
            }
            else
            {
                _context.Subscriptions.Add(new Subscription
                {
                    AppUserId = request.UserId,
                    ChannalId = request.ChannelId
                });
                isSubscribed = true;
            }

            await _context.SaveChangesAsync(cancellationToken);

            var count = await _context.Subscriptions
                .CountAsync(s => s.ChannalId == request.ChannelId, cancellationToken);

            return new ToggleSubscribeResult
            {
                Success = true,
                IsSubscribed = isSubscribed,
                SubscribersCount = count
            };
        }
    }
}
