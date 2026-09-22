using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Entities;

namespace youtube.Modules.Channels.Data
{
    public class ChannelsDbContext : DbContext
    {
        public ChannelsDbContext(DbContextOptions<ChannelsDbContext> options) : base(options)
        {
        }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.ToTable("Subscriptions");
                entity.HasKey(s => new { s.AppUserId, s.ChannalId });

                entity.HasOne(s => s.Channel)
                      .WithMany(c => c.Subscriptions)
                      .HasForeignKey(s => s.ChannalId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.ToTable("Channals");
            });
        }
    }
}
