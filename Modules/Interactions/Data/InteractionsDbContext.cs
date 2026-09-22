using Microsoft.EntityFrameworkCore;
using youtube.Modules.Interactions.Entities;

namespace youtube.Modules.Interactions.Data
{
    public class InteractionsDbContext : DbContext
    {
        public InteractionsDbContext(DbContextOptions<InteractionsDbContext> options) : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<LikeDislike> LikesDislikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LikeDislike>(entity =>
            {
                entity.ToTable("LikesDislikes");
                entity.HasKey(l => new { l.AppUserId, l.VideoId });
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");
            });
        }
    }
}
