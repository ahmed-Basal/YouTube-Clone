using System;
using System.Collections.Generic;

namespace youtube.viewmodels.VideoVm
{
    public class VideoWatch_vm
    {
        public WatchVideoItem Video { get; set; }
        public WatchChannelItem Channel { get; set; }
        public int SubscribersCount { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsChannelOwner { get; set; }
        public int? CurrentUserId { get; set; }
        public IEnumerable<WatchCommentItem> Comments { get; set; } = new List<WatchCommentItem>();
        public IEnumerable<WatchVideoItem> RelatedVideos { get; set; } = new List<WatchVideoItem>();
    }

    public class WatchVideoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public string VideoUrl { get; set; }
        public int Views { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ChannelId { get; set; }
        public WatchChannelItem Channal { get; set; }
        public WatchCategoryItem Category { get; set; }
    }

    public class WatchChannelItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public int AppUserId { get; set; }
    }

    public class WatchCategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class WatchCommentItem
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public int AppUserId { get; set; }
        public string Content { get; set; }
        public DateTime PostAt { get; set; }
        public WatchUserItem AppUser { get; set; }
    }

    public class WatchUserItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}
