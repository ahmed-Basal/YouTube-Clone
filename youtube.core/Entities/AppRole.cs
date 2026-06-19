using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace youtube.core.Entities
{
    public  class AppRole:IdentityRole<int>
    {
        [Required]
        public string Name { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
