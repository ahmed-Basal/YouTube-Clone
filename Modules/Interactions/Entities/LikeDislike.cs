using System.ComponentModel.DataAnnotations.Schema;

namespace youtube.Modules.Interactions.Entities
{
    [Table("LikesDislikes")]
    public class LikeDislike
    {
        public int AppUserId { get; set; }
        public int VideoId { get; set; }
        public bool like { get; set; } = true;
    }
}
