using System;

namespace youtube.Modules.Interactions.Contracts
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public int AppUserId { get; set; }
        public string Content { get; set; }
        public DateTime PostAt { get; set; }
    }

    public class VideoReactionsDto
    {
        public int VideoId { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public bool? CurrentUserLike { get; set; } // true = liked, false = disliked, null = no reaction
    }
}
