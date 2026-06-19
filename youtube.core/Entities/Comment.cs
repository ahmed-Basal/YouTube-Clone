using System;
using System.Collections.Generic;
using System.Text;

namespace youtube.core.Entities
{
    public  class Comment:BaseEntitiy
    {
        public int AppUserId { get; set; }
        public int VideoId { get; set; }

        public string Content { get; set; }
        public DateTime PostAt { get; set; } = DateTime.UtcNow;

        public AppUser AppUser { get;set; }
        public videos videos { get; set; }
    }
}

