using AspNetCoreHero.ToastNotification;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using SR.Hubs;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WebSellingCosmetics.Models;
using WebSellingCosmetics.Service;
using WebSellingCosmetics.Services;
using static WebSellingCosmetics.Service.MessageService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<WebMyPhamContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("WebSellingCosmetics")));
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });

builder.Services.AddSingleton<IUserConnectionService, UserConnectionService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "WebSellingCosmetics.Cookies";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.LoginPath = "/Admin/Account/Login";
        options.AccessDeniedPath = "/Admin/Account/Login";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;

    });
builder.Services.AddSingleton<IVnPayService, VnPayService>();
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

app.UseSession();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "UpLoads")
        ),
    RequestPath = "/contents"
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chatHub");
app.Run();
