using System;
using System.Collections.Generic;
using System.Text;

namespace youtube.core.Entities
{
    public  class Channal: BaseEntitiy
    {
        public string Name { get; set; }
        public string About { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int AppUserId { get; set; }

        public AppUser AppUser { get; set; }
        public ICollection<SubScription> subScriptions { get; set; }
    }
}
