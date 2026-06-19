using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using youtube.core.Entities;

namespace youtube.DataAccess.Data.config
{
    public class LikedDislikedConfig : IEntityTypeConfiguration<likesDislikes>
    {
        public void Configure(EntityTypeBuilder<likesDislikes> builder)
        {
            // defining the primary key which is a combination of both AppUserId and VideoId
            builder.HasKey(x => new { x.AppUserId, x.VideoId });

            builder.HasOne(a => a.AppUser)
                   .WithMany(c => c.likesDislikes)
                   .HasForeignKey(c => c.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Video)
                   .WithMany(c => c.LikeDislikes)
                   .HasForeignKey(c => c.VideoId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
