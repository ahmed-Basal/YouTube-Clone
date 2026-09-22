using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using youtube.SharedKernel;

namespace youtube.Modules.Videos.Entities
{
    [Table("videos")]
    public class Video : BaseEntity
    {
        [Required]
        public string ThumbnailUrl { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string VideoUrl { get; set; }

        public int Views { get; set; } = 0;

        public int CategoryId { get; set; }

        public int ChannelId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
