using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Contracts;
using youtube.Modules.Channels.Data;
using youtube.SharedKernel;

namespace youtube.Modules.Channels.Queries
{
    public class ChannelQueryHandlers :
        IRequestHandler<GetChannelByUserIdQuery, Result<ChannelSummaryDto>>,
        IRequestHandler<GetChannelSummaryQuery, Result<ChannelSummaryDto>>,
        IRequestHandler<VerifyChannelOwnershipQuery, bool>,
        IRequestHandler<IsSubscribedQuery, bool>
    {
        private readonly ChannelsDbContext _context;

        public ChannelQueryHandlers(ChannelsDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ChannelSummaryDto>> Handle(GetChannelByUserIdQuery request, CancellationToken cancellationToken)
        {
            var channel = await _context.Channels
                .AsNoTracking()
                .Include(c => c.Subscriptions)
                .FirstOrDefaultAsync(c => c.AppUserId == request.UserId, cancellationToken);

            if (channel == null)
            {
                return Result<ChannelSummaryDto>.Failure("Channel not found", 404);
            }

            return Result<ChannelSummaryDto>.Success(new ChannelSummaryDto
            {
                Id = channel.Id,
                Name = channel.Name,
                About = channel.About,
                AppUserId = channel.AppUserId,
                CreatedAt = channel.CreatedAt,
                SubscribersCount = channel.Subscriptions.Count
            });
        }

        public async Task<Result<ChannelSummaryDto>> Handle(GetChannelSummaryQuery request, CancellationToken cancellationToken)
        {
            var channel = await _context.Channels
                .AsNoTracking()
                .Include(c => c.Subscriptions)
                .FirstOrDefaultAsync(c => c.Id == request.ChannelId, cancellationToken);

            if (channel == null)
            {
                return Result<ChannelSummaryDto>.Failure("Channel not found", 404);
            }

            return Result<ChannelSummaryDto>.Success(new ChannelSummaryDto
            {
                Id = channel.Id,
                Name = channel.Name,
                About = channel.About,
                AppUserId = channel.AppUserId,
                CreatedAt = channel.CreatedAt,
                SubscribersCount = channel.Subscriptions.Count
            });
        }

        public async Task<bool> Handle(VerifyChannelOwnershipQuery request, CancellationToken cancellationToken)
        {
            return await _context.Channels
                .AnyAsync(c => c.Id == request.ChannelId && c.AppUserId == request.UserId, cancellationToken);
        }

        public async Task<bool> Handle(IsSubscribedQuery request, CancellationToken cancellationToken)
        {
            return await _context.Subscriptions
                .AnyAsync(s => s.ChannalId == request.ChannelId && s.AppUserId == request.UserId, cancellationToken);
        }
    }
}
