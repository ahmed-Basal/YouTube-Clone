using Microsoft.AspNetCore.Identity;

namespace youtube.Modules.Users.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
