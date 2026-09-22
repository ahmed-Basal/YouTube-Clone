using System;

namespace youtube.viewmodels
{
    public class VideoCard_vm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Views { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CategoryId { get; set; }
        public int ChannelId { get; set; }
        public VideoCardChannel Channal { get; set; }
        public VideoCardCategory Category { get; set; }
    }

    public class VideoCardChannel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class VideoCardCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
