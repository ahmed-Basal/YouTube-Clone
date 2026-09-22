using System.Collections.Generic;
using youtube.Modules.Videos.Contracts;

namespace youtube.viewmodels
{
    public class AdminDashboard_vm
    {
        public int TotalVideos { get; set; }
        public int TotalChannels { get; set; }
        public int TotalCategories { get; set; }
        public int TotalComments { get; set; }
        public long TotalViews { get; set; }
        public int TotalUsers { get; set; }

        public IEnumerable<VideoCard_vm> RecentVideos { get; set; }
        public IEnumerable<CategoryDto> Categories { get; set; }
    }
}
