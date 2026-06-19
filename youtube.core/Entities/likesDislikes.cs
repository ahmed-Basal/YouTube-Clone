using System;
using System.Collections.Generic;
using System.Text;

namespace youtube.core.Entities
{
    public  class likesDislikes
    {
        public int AppUserId { get; set; }
        public int VideoId { get; set; }
        public bool like { get; set; } = true;
        public AppUser AppUser { get; set; }
        public videos Video { get; set; } = new videos();
    }
}
