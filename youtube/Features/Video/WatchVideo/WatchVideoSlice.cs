using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.Modules.Interactions.Data;
using youtube.Modules.Users.Data;
using youtube.Modules.Videos.Data;
using youtube.viewmodels.VideoVm;

namespace youtube.Features.Video.WatchVideo
{
    public record WatchVideoQuery(int VideoId, int? CurrentUserId) : IRequest<VideoWatch_vm>;

    public class WatchVideoHandler : IRequestHandler<WatchVideoQuery, VideoWatch_vm>
    {
        private readonly VideosDbContext _videosContext;
        private readonly ChannelsDbContext _channelsContext;
        private readonly InteractionsDbContext _interactionsContext;
        private readonly UsersDbContext _usersContext;

        public WatchVideoHandler(
            VideosDbContext videosContext,
            ChannelsDbContext channelsContext,
            InteractionsDbContext interactionsContext,
            UsersDbContext usersContext)
        {
            _videosContext = videosContext;
            _channelsContext = channelsContext;
            _interactionsContext = interactionsContext;
            _usersContext = usersContext;
        }

        public async Task<VideoWatch_vm> Handle(WatchVideoQuery request, CancellationToken cancellationToken)
        {
            var video = await _videosContext.Videos
                .Include(v => v.Category)
                .FirstOrDefaultAsync(v => v.Id == request.VideoId, cancellationToken);

            if (video == null)
            {
                return null;
            }

            // Increment view count
            video.Views++;
            await _videosContext.SaveChangesAsync(cancellationToken);

            var channel = await _channelsContext.Channels
                .FirstOrDefaultAsync(c => c.Id == video.ChannelId, cancellationToken);

            var subscribersCount = await _channelsContext.Subscriptions
                .CountAsync(s => s.ChannalId == video.ChannelId, cancellationToken);

            bool isSubscribed = false;
            bool isLiked = false;
            bool isDisliked = false;

            if (request.CurrentUserId.HasValue)
            {
                isSubscribed = await _channelsContext.Subscriptions
                    .AnyAsync(s => s.ChannalId == video.ChannelId && s.AppUserId == request.CurrentUserId.Value, cancellationToken);

                var userReaction = await _interactionsContext.LikesDislikes
                    .FirstOrDefaultAsync(l => l.VideoId == video.Id && l.AppUserId == request.CurrentUserId.Value, cancellationToken);

                if (userReaction != null)
                {
                    isLiked = userReaction.like;
                    isDisliked = !userReaction.like;
                }
            }

            var likesCount = await _interactionsContext.LikesDislikes
                .CountAsync(l => l.VideoId == video.Id && l.like, cancellationToken);

            var dislikesCount = await _interactionsContext.LikesDislikes
                .CountAsync(l => l.VideoId == video.Id && !l.like, cancellationToken);

            var comments = await _interactionsContext.Comments
                .Where(c => c.VideoId == video.Id)
                .OrderByDescending(c => c.PostAt)
                .ToListAsync(cancellationToken);

            var commentUserIds = comments.Select(c => c.AppUserId).Distinct().ToList();
            var users = await _usersContext.Users
                .Where(u => commentUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, cancellationToken);

            var commentItems = comments.Select(c => new WatchCommentItem
            {
                Id = c.Id,
                VideoId = c.VideoId,
                AppUserId = c.AppUserId,
                Content = c.Content,
                PostAt = c.PostAt,
                AppUser = users.TryGetValue(c.AppUserId, out var u) ? new WatchUserItem
                {
                    Id = u.Id,
                    Name = u.Name,
                    UserName = u.UserName
                } : new WatchUserItem { Id = c.AppUserId, Name = "User", UserName = "user" }
            }).ToList();

            var relatedVideosDb = await _videosContext.Videos
                .Include(v => v.Category)
                .Where(v => v.Id != video.Id)
                .OrderByDescending(v => v.CreatedAt)
                .Take(8)
                .ToListAsync(cancellationToken);

            var relatedChannelIds = relatedVideosDb.Select(v => v.ChannelId).Distinct().ToList();
            var relatedChannels = await _channelsContext.Channels
                .Where(c => relatedChannelIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, cancellationToken);

            var relatedVideoItems = relatedVideosDb.Select(v => new WatchVideoItem
            {
                Id = v.Id,
                Title = v.Title,
                Description = v.Description,
                ThumbnailUrl = v.ThumbnailUrl,
                VideoUrl = v.VideoUrl,
                Views = v.Views,
                CreatedAt = v.CreatedAt,
                ChannelId = v.ChannelId,
                Channal = relatedChannels.TryGetValue(v.ChannelId, out var ch) ? new WatchChannelItem
                {
                    Id = ch.Id,
                    Name = ch.Name,
                    About = ch.About,
                    AppUserId = ch.AppUserId
                } : null,
                Category = v.Category != null ? new WatchCategoryItem
                {
                    Id = v.Category.Id,
                    Name = v.Category.Name
                } : null
            }).ToList();

            var watchChannelItem = channel != null ? new WatchChannelItem
            {
                Id = channel.Id,
                Name = channel.Name,
                About = channel.About,
                AppUserId = channel.AppUserId
            } : null;

            return new VideoWatch_vm
            {
                Video = new WatchVideoItem
                {
                    Id = video.Id,
                    Title = video.Title,
                    Description = video.Description,
                    ThumbnailUrl = video.ThumbnailUrl,
                    VideoUrl = video.VideoUrl,
                    Views = video.Views,
                    CreatedAt = video.CreatedAt,
                    ChannelId = video.ChannelId,
                    Channal = watchChannelItem,
                    Category = video.Category != null ? new WatchCategoryItem
                    {
                        Id = video.Category.Id,
                        Name = video.Category.Name
                    } : null
                },
                Channel = watchChannelItem,
                SubscribersCount = subscribersCount,
                IsSubscribed = isSubscribed,
                IsLiked = isLiked,
                IsDisliked = isDisliked,
                LikesCount = likesCount,
                DislikesCount = dislikesCount,
                CommentsCount = comments.Count,
                IsChannelOwner = request.CurrentUserId.HasValue && channel != null && channel.AppUserId == request.CurrentUserId.Value,
                CurrentUserId = request.CurrentUserId,
                Comments = commentItems,
                RelatedVideos = relatedVideoItems
            };
        }
    }
}
