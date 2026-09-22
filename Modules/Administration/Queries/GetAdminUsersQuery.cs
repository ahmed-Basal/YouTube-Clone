using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Administration.DTOs;
using youtube.Modules.Channels.Data;
using youtube.Modules.Users.Data;
using youtube.Modules.Users.Entities;
using youtube.Modules.Videos.Data;

namespace youtube.Modules.Administration.Queries
{
    public record GetAdminUsersQuery(int CurrentUserId) : IRequest<List<AdminUserDto>>;

    public class GetAdminUsersHandler : IRequestHandler<GetAdminUsersQuery, List<AdminUserDto>>
    {
        private readonly UsersDbContext _usersContext;
        private readonly ChannelsDbContext _channelsContext;
        private readonly VideosDbContext _videosContext;
        private readonly UserManager<AppUser> _userManager;

        public GetAdminUsersHandler(
            UsersDbContext usersContext,
            ChannelsDbContext channelsContext,
            VideosDbContext videosContext,
            UserManager<AppUser> userManager)
        {
            _usersContext = usersContext;
            _channelsContext = channelsContext;
            _videosContext = videosContext;
            _userManager = userManager;
        }

        public async Task<List<AdminUserDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _usersContext.Users
                .AsNoTracking()
                .OrderByDescending(u => u.CreateAt)
                .ToListAsync(cancellationToken);

            var userIds = users.Select(u => u.Id).ToList();

            var channels = await _channelsContext.Channels
                .AsNoTracking()
                .Where(c => userIds.Contains(c.AppUserId))
                .ToDictionaryAsync(c => c.AppUserId, cancellationToken);

            var channelIds = channels.Values.Select(c => c.Id).ToList();

            var videoCounts = await _videosContext.Videos
                .AsNoTracking()
                .Where(v => channelIds.Contains(v.ChannelId))
                .GroupBy(v => v.ChannelId)
                .Select(g => new { ChannelId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ChannelId, x => x.Count, cancellationToken);

            var result = new List<AdminUserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                channels.TryGetValue(user.Id, out var channel);
                var videoCount = 0;
                if (channel != null && videoCounts.TryGetValue(channel.Id, out var count))
                {
                    videoCount = count;
                }

                result.Add(new AdminUserDto
                {
                    Id = user.Id,
                    Name = !string.IsNullOrWhiteSpace(user.Name) ? user.Name : (user.UserName ?? "User"),
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    CreatedAt = user.CreateAt,
                    Roles = roles,
                    ChannelName = channel?.Name,
                    ChannelId = channel?.Id,
                    VideoCount = videoCount,
                    IsCurrentAdmin = user.Id == request.CurrentUserId
                });
            }

            return result;
        }
    }
}
