using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Users.Entities;

namespace youtube.Modules.Users.Data
{
    public class UsersDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Default ASP.NET Identity tables (AspNetUsers, AspNetRoles, etc.)
        }
    }
}
