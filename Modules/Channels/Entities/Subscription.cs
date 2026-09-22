using System.ComponentModel.DataAnnotations.Schema;

namespace youtube.Modules.Channels.Entities
{
    [Table("SubScription")]
    public class Subscription
    {
        public int AppUserId { get; set; }
        public int ChannalId { get; set; }

        public Channel Channel { get; set; }
    }
}
