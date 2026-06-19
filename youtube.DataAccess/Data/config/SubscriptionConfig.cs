using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using youtube.core.Entities;

namespace youtube.DataAccess.Data.config
{
    public class SubscriptionConfig : IEntityTypeConfiguration<SubScription>
    {
        void IEntityTypeConfiguration<SubScription>.Configure(EntityTypeBuilder<SubScription> builder)
        {
            builder.HasKey(x => new { x.AppUserId, x.ChannalId });

            builder.HasOne(a => a.AppUser)
                   .WithMany(c => c.subScriptions)
                   .HasForeignKey(c => c.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.channal)
                   .WithMany(c => c.subScriptions)
                   .HasForeignKey(c => c.ChannalId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
