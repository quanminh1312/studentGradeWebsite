using doancoso.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SIUDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") + ";TrustServerCertificate=True"));
builder.Services.AddControllersWithViews();
builder.Services.AddIdentity<User, IdentityRole>(idd =>
{
    idd.Password.RequireDigit = false;
    idd.Password.RequireLowercase = false;
    idd.Password.RequireUppercase = false;
    idd.Password.RequireNonAlphanumeric = false;
    idd.Password.RequiredLength = 4;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<SIUDBContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home";
});
builder.Services.AddSession(
    option =>
    {
        option.IdleTimeout = TimeSpan.FromMinutes(120);
        option.Cookie.HttpOnly = true;
        option.Cookie.IsEssential = true;
    });
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Student",
    pattern: "{area:exists}/{controller=Student}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Teacher",
    pattern: "{area:exists}/{controller=Teacher}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
