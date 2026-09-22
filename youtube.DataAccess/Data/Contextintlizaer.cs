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
    public static class Contextintlizaer
    {
        public static async Task Initialize(Context context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            // Seed Roles
            foreach (var roleName in SD.Roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new AppRole { Name = roleName };
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to create role '{roleName}': {errors}");
                    }
                }
            }

            // Seed Users
            try 
            {
                var adminUser = await userManager.FindByEmailAsync("adminexample@gmail.com");
                if (adminUser == null)
                {
                    var admin = new AppUser
                    {
                        Name = "admin",
                        Email = "adminexample@gmail.com",
                        UserName = "admin"
                    };

                    var resultAdmin = await userManager.CreateAsync(admin, "password123");
                    if (!resultAdmin.Succeeded)
                    {
                        var errors = string.Join(", ", resultAdmin.Errors.Select(e => e.Description));
                        throw new Exception($"Failed to create admin user: {errors}");
                    }
                    await userManager.AddToRolesAsync(admin, [SD.AdminRole, SD.UserRole, SD.ModerateRole]);
                }
                else
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                    await userManager.ResetPasswordAsync(adminUser, token, "password123");
                    
                    // Also ensure roles are set just in case
                    if (!await userManager.IsInRoleAsync(adminUser, SD.AdminRole))
                    {
                        await userManager.AddToRolesAsync(adminUser, [SD.AdminRole, SD.UserRole, SD.ModerateRole]);
                    }
                }

                if (await userManager.FindByEmailAsync("johan@gmail.com") == null)
                {
                    var johan = new AppUser
                    {
                        Name = "johan",
                        Email = "johan@gmail.com",
                        UserName = "johan"
                    };
                    await userManager.CreateAsync(johan, "johan123");
                    await userManager.AddToRoleAsync(johan, SD.UserRole);
                }

                if (await userManager.FindByEmailAsync("mary@gmail.com") == null && await userManager.FindByNameAsync("mary") == null)
                {
                    var mary = new AppUser
                    {
                        Name = "mary",
                        Email = "mary@gmail.com",
                        UserName = "mary",
                        CreateAt = DateTime.UtcNow
                    };
                    var result = await userManager.CreateAsync(mary, "Mary@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(mary, SD.ModerateRole);
                    }
                }

                // Seed Categories
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(new List<Category>
                    {
                        new Category { Name = "Music" },
                        new Category { Name = "Sports" },
                        new Category { Name = "Gaming" },
                        new Category { Name = "News" },
                        new Category { Name = "Movies" }
                    });
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // This will help capture errors in seeding specifically
                throw new Exception("Error during user seeding", ex);
            }
        }
    }
}