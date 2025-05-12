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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
