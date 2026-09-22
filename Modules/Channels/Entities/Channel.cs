using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using youtube.SharedKernel;

namespace youtube.Modules.Channels.Entities
{
    [Table("Channals")]
    public class Channel : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public string About { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int AppUserId { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
