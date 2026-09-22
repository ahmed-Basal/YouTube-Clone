using System.Collections.Generic;
using youtube.Modules.Videos.Contracts;

namespace youtube.viewmodels
{
    public class HomeFeed_vm
    {
        public IEnumerable<VideoCard_vm> Videos { get; set; } = new List<VideoCard_vm>();
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public int? ActiveCategoryId { get; set; }
        public string SearchQuery { get; set; }
    }
}
