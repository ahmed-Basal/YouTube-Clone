using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using youtube.DataAccess.Data;
using youtube.core.Entities;
using youtube.Extentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

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
    // 1. بنعمل Scope مؤقت عشان نقدر نسحب الخدمات الـ Scoped بأمان
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        // 2. بنسحب الـ Context بتاع قاعدة البيانات (استخدمنا GetRequiredService للأمان)
        var context = services.GetRequiredService<Context>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

        // 3. بنشغل كلاس الـ Initializer المسؤول عن الـ Migration والـ Seed بيانات
        await Contextintlizaer.Initialize(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        // 4. لو حصلت أي كارثة في الخطوات اللي فوق، بنمسكها ونطبعها في الـ Console
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing or seeding the database.");
    }
}