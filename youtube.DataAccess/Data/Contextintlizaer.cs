using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UTlity;
using youtube.core.Entities;

namespace youtube.DataAccess.Data
{
    public static   class Contextintlizaer
    {
        public static async Task Initialize(Context context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            // Seed Roles
            if (!context.Roles.Any())
            {
                foreach (var role in SD.Roles)
                {
                    await roleManager.CreateAsync(new AppRole { Name = role });
                }
            }

            // Seed Admin User
            if (!userManager.Users.Any())
            {
                var admin = new AppUser { 
                    name="admin",
                    Email="adminexample@gmail.com",
                    UserName="admin"
                };

                await userManager.CreateAsync(admin, "password123");
                await userManager.AddToRolesAsync(admin, [SD.AdminRole, SD.UserRole, SD.ModerateRole]);
            }
        }
    }
}
