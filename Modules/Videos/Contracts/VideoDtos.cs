namespace youtube.Modules.Videos.Contracts
{
    public class VideoSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public string VideoUrl { get; set; }
        public int Views { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ChannelId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
