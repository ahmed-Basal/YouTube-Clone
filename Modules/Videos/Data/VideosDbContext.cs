using Microsoft.EntityFrameworkCore;
using youtube.Modules.Videos.Entities;

namespace youtube.Modules.Videos.Data
{
    public class VideosDbContext : DbContext
    {
        public VideosDbContext(DbContextOptions<VideosDbContext> options) : base(options)
        {
        }

        public DbSet<Video> Videos { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Video>(entity =>
            {
                entity.ToTable("videos");
                entity.HasOne(v => v.Category)
                      .WithMany()
                      .HasForeignKey(v => v.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
            });
        }
    }
}
