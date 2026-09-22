using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;

namespace youtube.Modules.Channels.Commands
{
    public record EditChannelCommand(int UserId, string Name, string About) : IRequest<bool>;

    public class EditChannelHandler : IRequestHandler<EditChannelCommand, bool>
    {
        private readonly ChannelsDbContext _context;

        public EditChannelHandler(ChannelsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(EditChannelCommand request, CancellationToken cancellationToken)
        {
            var channel = await _context.Channels
                .FirstOrDefaultAsync(x => x.AppUserId == request.UserId, cancellationToken);

            if (channel == null)
            {
                return false;
            }

            channel.Name = request.Name;
            channel.About = request.About;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
