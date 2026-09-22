using System;
using System.Collections.Generic;

namespace youtube.Modules.Administration.DTOs
{
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public string ChannelName { get; set; }
        public int? ChannelId { get; set; }
        public int VideoCount { get; set; }
        public bool IsCurrentAdmin { get; set; }
    }

    public class AdminCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
