using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace youtube.core.Entities
{
    public  class videos:BaseEntitiy
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

        // Navigations
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [ForeignKey("ChannelId")]
        public Channal Channal { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<likesDislikes> LikeDislikes { get; set; }
        
    }
}
