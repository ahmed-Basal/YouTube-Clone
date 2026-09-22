using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using youtube.Extentions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddRazorOptions(options =>
    {
        options.ViewLocationExpanders.Add(new youtube.Extentions.FeatureViewLocationExpander());
    });
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Applicationbuilder();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await InitializeContext();
app.Run();

async Task InitializeContext()
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var usersContext = services.GetRequiredService<youtube.Modules.Users.Data.UsersDbContext>();
        var channelsContext = services.GetRequiredService<youtube.Modules.Channels.Data.ChannelsDbContext>();
        var videosContext = services.GetRequiredService<youtube.Modules.Videos.Data.VideosDbContext>();
        var userManager = services.GetRequiredService<UserManager<youtube.Modules.Users.Entities.AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<youtube.Modules.Users.Entities.AppRole>>();

        await youtube.DatabaseInitializer.InitializeAsync(usersContext, channelsContext, videosContext, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing or seeding the database.");
    }
}