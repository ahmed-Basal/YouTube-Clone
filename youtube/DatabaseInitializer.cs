using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Channels.Data;
using youtube.Modules.Channels.Entities;
using youtube.Modules.Users.Data;
using youtube.Modules.Users.Entities;
using youtube.Modules.Videos.Data;
using youtube.Modules.Videos.Entities;
using youtube.SharedKernel;

namespace youtube
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(
            UsersDbContext usersContext,
            ChannelsDbContext channelsContext,
            VideosDbContext videosContext,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
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
                    if (resultAdmin.Succeeded)
                    {
                        await userManager.AddToRolesAsync(admin, [SD.AdminRole, SD.UserRole, SD.ModerateRole]);
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
                    var res = await userManager.CreateAsync(johan, "johan123");
                    if (res.Succeeded)
                    {
                        await userManager.AddToRoleAsync(johan, SD.UserRole);
                    }
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
                if (!await videosContext.Categories.AnyAsync())
                {
                    videosContext.Categories.AddRange(new List<Category>
                    {
                        new() { Name = "Music" },
                        new() { Name = "Sports" },
                        new() { Name = "Gaming" },
                        new() { Name = "News" },
                        new() { Name = "Movies" }
                    });
                    await videosContext.SaveChangesAsync();
                }

                // Ensure Every User has a Channel
                var allUsers = await userManager.Users.ToListAsync();
                foreach (var u in allUsers)
                {
                    var existingChannel = await channelsContext.Channels.FirstOrDefaultAsync(c => c.AppUserId == u.Id);
                    if (existingChannel == null)
                    {
                        var channelName = u.Name ?? u.UserName ?? "Creator";
                        if (u.Email == "adminexample@gmail.com") channelName = "VidPulse Official";
                        else if (u.UserName == "johan") channelName = "Johan Gaming";
                        else if (u.UserName == "mary") channelName = "Mary Tech Hub";

                        var newChannel = new Channel
                        {
                            Name = channelName,
                            About = $"Welcome to {channelName}! Discover trending videos, gaming clips, tutorials, and lifestyle.",
                            AppUserId = u.Id,
                            CreatedAt = DateTime.UtcNow
                        };
                        channelsContext.Channels.Add(newChannel);
                    }
                }
                await channelsContext.SaveChangesAsync();

                var channels = await channelsContext.Channels.ToListAsync();
                var categories = await videosContext.Categories.ToListAsync();
                var defaultCat = categories.FirstOrDefault() ?? new Category { Name = "General" };

                var verifiedVideos = new[]
                {
                    new {
                        Title = "Last To Take Hand Off Jet, Keeps It! - MrBeast",
                        Description = "I gave away a private jet to whoever held their hand on it the longest! Extreme challenge with MrBeast and the crew.",
                        VideoUrl = "https://www.youtube.com/watch?v=kX3nB4PpJko",
                        ThumbnailUrl = "https://img.youtube.com/vi/kX3nB4PpJko/hqdefault.jpg",
                        CategoryName = "Gaming",
                        Views = 18450000
                    },
                    new {
                        Title = "Full Stack Web Development for Beginners (Full Course)",
                        Description = "Learn full stack web development from scratch with HTML, CSS, JavaScript, React, Node.js, and MongoDB in this comprehensive crash course by freeCodeCamp.",
                        VideoUrl = "https://www.youtube.com/watch?v=nu_pCVPKzTk",
                        ThumbnailUrl = "https://img.youtube.com/vi/nu_pCVPKzTk/hqdefault.jpg",
                        CategoryName = "News",
                        Views = 4520000
                    },
                    new {
                        Title = "COSTA RICA IN 4K 60fps HDR (ULTRA HD Wildlife)",
                        Description = "Experience Costa Rica like never before in stunning 4K 60fps Ultra HD. Majestic wildlife, lush rainforests, and vibrant tropical beaches.",
                        VideoUrl = "https://www.youtube.com/watch?v=LXb3EKWsInQ",
                        ThumbnailUrl = "https://img.youtube.com/vi/LXb3EKWsInQ/hqdefault.jpg",
                        CategoryName = "Sports",
                        Views = 24900000
                    },
                    new {
                        Title = "React Tutorial for Beginners - Learn React in 1 Hour",
                        Description = "Master modern React 18 fundamentals step by step. Components, props, state, hooks, and clean architecture explained by Programming with Mosh.",
                        VideoUrl = "https://www.youtube.com/watch?v=SqcY0GlETPk",
                        ThumbnailUrl = "https://img.youtube.com/vi/SqcY0GlETPk/hqdefault.jpg",
                        CategoryName = "Movies",
                        Views = 3280000
                    },
                    new {
                        Title = "Queen – Bohemian Rhapsody (Official Video Remastered)",
                        Description = "The official, fully remastered 4K music video for Bohemian Rhapsody by Queen. One of the greatest rock anthems in history.",
                        VideoUrl = "https://www.youtube.com/watch?v=fJ9rUzIMcZQ",
                        ThumbnailUrl = "https://img.youtube.com/vi/fJ9rUzIMcZQ/hqdefault.jpg",
                        CategoryName = "Music",
                        Views = 1680000000
                    },
                    new {
                        Title = "Luis Fonsi - Despacito ft. Daddy Yankee",
                        Description = "Official music video for Despacito by Luis Fonsi featuring Daddy Yankee. The global multi-platinum Latin sensation.",
                        VideoUrl = "https://www.youtube.com/watch?v=kJQP7kiw5Fk",
                        ThumbnailUrl = "https://img.youtube.com/vi/kJQP7kiw5Fk/hqdefault.jpg",
                        CategoryName = "Music",
                        Views = 28000000
                    },
                    new {
                        Title = "Rick Astley - Never Gonna Give You Up (Official Video) (4K Remaster)",
                        Description = "The iconic official video for “Never Gonna Give You Up” by Rick Astley. Restored in stunning 4K high definition.",
                        VideoUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                        ThumbnailUrl = "https://img.youtube.com/vi/dQw4w9WgXcQ/hqdefault.jpg",
                        CategoryName = "Music",
                        Views = 1520000000
                    },
                    new {
                        Title = "lofi hip hop radio 📚 beats to relax/study to",
                        Description = "Peaceful lofi hip hop beats to study, relax, and code to. Ambient vibes streamed by Lofi Girl.",
                        VideoUrl = "https://www.youtube.com/watch?v=jfKfPfyJRdk",
                        ThumbnailUrl = "https://img.youtube.com/vi/jfKfPfyJRdk/hqdefault.jpg",
                        CategoryName = "Music",
                        Views = 68400000
                    },
                    new {
                        Title = "Solidity, Blockchain, and Smart Contract Course",
                        Description = "Learn smart contract development, DeFi fundamentals, Solidity, and Web3 security in this complete developer bootcamp.",
                        VideoUrl = "https://www.youtube.com/watch?v=M576WGiDBdQ",
                        ThumbnailUrl = "https://img.youtube.com/vi/M576WGiDBdQ/hqdefault.jpg",
                        CategoryName = "News",
                        Views = 1890000
                    },
                    new {
                        Title = "Fireplace Ambience – Cozy Fire for Relaxation & Background Viewing",
                        Description = "Relaxing burning wood fireplace with soothing crackling fire sounds. Perfect for study, sleep, and ambient warmth.",
                        VideoUrl = "https://www.youtube.com/watch?v=L_LUpnjgPso",
                        ThumbnailUrl = "https://img.youtube.com/vi/L_LUpnjgPso/hqdefault.jpg",
                        CategoryName = "Movies",
                        Views = 45100000
                    }
                };

                var existingVideos = await videosContext.Videos.ToListAsync();
                for (int i = 0; i < verifiedVideos.Length; i++)
                {
                    var data = verifiedVideos[i];
                    var assignedChannel = channels[i % channels.Count];
                    var assignedCategory = categories.FirstOrDefault(c => c.Name.Equals(data.CategoryName, StringComparison.OrdinalIgnoreCase)) ?? defaultCat;

                    if (i < existingVideos.Count)
                    {
                        var v = existingVideos[i];
                        v.Title = data.Title;
                        v.Description = data.Description;
                        v.VideoUrl = data.VideoUrl;
                        v.ThumbnailUrl = data.ThumbnailUrl;
                        v.ChannelId = assignedChannel.Id;
                        v.CategoryId = assignedCategory.Id;
                        v.Views = data.Views;
                    }
                    else
                    {
                        var newVideo = new Video
                        {
                            Title = data.Title,
                            Description = data.Description,
                            VideoUrl = data.VideoUrl,
                            ThumbnailUrl = data.ThumbnailUrl,
                            ChannelId = assignedChannel.Id,
                            CategoryId = assignedCategory.Id,
                            Views = data.Views,
                            CreatedAt = DateTime.UtcNow.AddDays(-(i + 1))
                        };
                        videosContext.Videos.Add(newVideo);
                    }
                }
                await videosContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error during database seeding", ex);
            }
        }
    }
}
