namespace youtube.Modules.Channels.Contracts
{
    public class ChannelSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public int AppUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SubscribersCount { get; set; }
    }
}
