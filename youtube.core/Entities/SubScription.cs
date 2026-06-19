using System;
using System.Collections.Generic;
using System.Text;

namespace youtube.core.Entities
{
    public  class SubScription
    {
        public int AppUserId { get; set; }
        public int ChannalId { get; set; }

        public AppUser AppUser { get; set; }
        public Channal channal { get; set; }


    }
}
