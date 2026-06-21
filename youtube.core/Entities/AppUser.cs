using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace youtube.core.Entities
{
    public  class AppUser:IdentityUser<int>
    {
        [Required]
        public string Name { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public Channal channal { get; set; }
        public ICollection<Comment> coment { get; set; }
        public ICollection<SubScription> subScriptions { get; set;}
        public ICollection<likesDislikes> likesDislikes { get; set; }
    }
}
