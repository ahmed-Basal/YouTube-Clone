using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.Modules.Channels.Entities;
using youtube.viewmodels.account;

namespace youtube.Features.Channal
{
    // ================= Get Channel =================
    public record GetChannelQuery(int UserId) : IRequest<ChannelAddEdit_vm>;

    public class GetChannelHandler : IRequestHandler<GetChannelQuery, ChannelAddEdit_vm>
    {
        private readonly ChannelsDbContext _context;

        public GetChannelHandler(ChannelsDbContext context)
        {
            _context = context;
        }

        public async Task<ChannelAddEdit_vm> Handle(GetChannelQuery request, CancellationToken cancellationToken)
        {
            var model = new ChannelAddEdit_vm();
            var channel = await _context.Channels
                .Include(c => c.Subscriptions)
                .FirstOrDefaultAsync(x => x.AppUserId == request.UserId, cancellationToken);

            if (channel != null)
            {
                model.Name = channel.Name;
                model.About = channel.About;
                model.SubscribersCount = channel.Subscriptions?.Count ?? 0;
            }

            return model;
        }
    }

    // ================= Create Channel =================
    public record CreateChannelCommand(int UserId, ChannelAddEdit_vm Model) : IRequest<(bool Success, string ErrorKey, string ErrorMessage)>;

    public class CreateChannelHandler : IRequestHandler<CreateChannelCommand, (bool Success, string ErrorKey, string ErrorMessage)>
    {
        private readonly ChannelsDbContext _context;

        public CreateChannelHandler(ChannelsDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string ErrorKey, string ErrorMessage)> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
        {
            var nameLower = request.Model.Name.Trim().ToLower();
            var channelNameExists = await _context.Channels
                .AnyAsync(x => x.Name.ToLower() == nameLower, cancellationToken);

            if (channelNameExists)
            {
                return (false, "Name", $"Channel name of {request.Model.Name} is taken. Please try another name");
            }

            var channelToAdd = new Channel
            {
                AppUserId = request.UserId,
                Name = request.Model.Name.Trim(),
                About = request.Model.About?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Channels.Add(channelToAdd);
            await _context.SaveChangesAsync(cancellationToken);

            return (true, null, null);
        }
    }

    // ================= Edit Channel =================
    public record EditChannelCommand(int UserId, ChannelAddEdit_vm Model) : IRequest<bool>;

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

            channel.Name = request.Model.Name;
            channel.About = request.Model.About;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
