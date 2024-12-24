using KuaforUygulamasi.Data;
using KuaforUygulamasi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısını yapılandır
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kimlik doğrulama servislerini ekle
builder.Services.AddDefaultIdentity<Kullanici>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Çerez yapılandırması
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Giriş sayfası yolu
    options.AccessDeniedPath = "/Account/AccessDenied"; // Erişim reddi sayfası
    options.SlidingExpiration = true; // Oturum geçerlilik süresi
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Oturum süresi
});

// MVC servisini ekle
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Hata sayfası ve HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // Kimlik doğrulama
app.UseAuthorization();  // Yetkilendirme

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
