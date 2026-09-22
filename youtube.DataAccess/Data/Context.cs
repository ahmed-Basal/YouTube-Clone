using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using youtube.core.Entities;
using youtube.DataAccess.Data.config;

namespace youtube.DataAccess.Data
{
    public class Context : IdentityDbContext<AppUser,AppRole,int>
    {
        public Context(DbContextOptions<Context>option):base(option)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Channal> Channals { get; set; }
        public DbSet<videos> videos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<SubScription> Subscriptions { get; set; }
        public DbSet<likesDislikes> LikesDislikes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.ApplyConfiguration(new CommentConfig());
            builder.ApplyConfiguration(new SubscriptionConfig());
            builder.ApplyConfiguration(new LikedDislikedConfig());
        }
    
    }


}
        
