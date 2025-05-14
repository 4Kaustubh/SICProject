using Microsoft.EntityFrameworkCore;
using SICProject.Helper;
using SICProject.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SicdbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("dbcs"),
    new MySqlServerVersion(new Version(8, 0, 34))));

// Register AutoMapper with the DI system
builder.Services.AddAutoMapper(typeof(ApplicationMapper)); // or specify all profiles via assembly scanning

//setup session in project
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(45); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use Secure cookies in production
});

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddAuthentication("CookieAuth")
           .AddCookie("CookieAuth", options =>
           {
               options.LoginPath = "/Home/login";
               options.AccessDeniedPath = "/Home/login";
           });

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

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
