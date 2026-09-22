using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.Modules.Channels.Entities;

namespace youtube.Modules.Channels.Commands
{
    public record CreateChannelCommand(int UserId, string Name, string About) : IRequest<CreateChannelResult>;

    public class CreateChannelResult
    {
        public bool Success { get; set; }
        public string ErrorKey { get; set; }
        public string ErrorMessage { get; set; }
        public int ChannelId { get; set; }
    }

    public class CreateChannelHandler : IRequestHandler<CreateChannelCommand, CreateChannelResult>
    {
        private readonly ChannelsDbContext _context;

        public CreateChannelHandler(ChannelsDbContext context)
        {
            _context = context;
        }

        public async Task<CreateChannelResult> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
        {
            var nameLower = request.Name?.Trim().ToLower();
            var channelNameExists = await _context.Channels
                .AnyAsync(x => x.Name.ToLower() == nameLower, cancellationToken);

            if (channelNameExists)
            {
                return new CreateChannelResult
                {
                    Success = false,
                    ErrorKey = "Name",
                    ErrorMessage = $"Channel name '{request.Name}' is taken. Please choose another name."
                };
            }

            var channel = new Channel
            {
                AppUserId = request.UserId,
                Name = request.Name?.Trim(),
                About = request.About?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Channels.Add(channel);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateChannelResult
            {
                Success = true,
                ChannelId = channel.Id
            };
        }
    }
}
