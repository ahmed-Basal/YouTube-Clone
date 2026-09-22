using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using youtube.SharedKernel;

namespace youtube.Modules.Interactions.Entities
{
    [Table("Comments")]
    public class Comment : BaseEntity
    {
        public int AppUserId { get; set; }
        public int VideoId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PostAt { get; set; } = DateTime.UtcNow;
    }
}
