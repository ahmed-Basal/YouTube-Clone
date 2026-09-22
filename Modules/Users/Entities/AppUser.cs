using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace youtube.Modules.Users.Entities
{
    public class AppUser : IdentityUser<int>
    {
        [Required]
        public string Name { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
