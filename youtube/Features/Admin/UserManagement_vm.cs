#nullable enable
using System;
using System.Collections.Generic;

namespace youtube.viewmodels
{
    public class UserManagement_vm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public string? ChannelName { get; set; }
        public int? ChannelId { get; set; }
        public int VideoCount { get; set; }
        public bool IsCurrentAdmin { get; set; }
    }
}
