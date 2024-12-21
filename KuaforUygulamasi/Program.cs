using KuaforUygulamasi.Data; // ApplicationDbContext için
using KuaforUygulamasi.Models; // Kullanici modeli için
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kimlik doğrulama için Identity'yi ekle
builder.Services.AddDefaultIdentity<Kullanici>(options =>
{
    options.Password.RequireDigit = true; // Şifre için sayı zorunluluğu
    options.Password.RequireLowercase = true; // Küçük harf zorunluluğu
    options.Password.RequireUppercase = true; // Büyük harf zorunluluğu
    options.Password.RequireNonAlphanumeric = false; // Alfasayısal olmayan karakter zorunlu değil
    options.Password.RequiredLength = 6; // Minimum şifre uzunluğu
})
    .AddRoles<IdentityRole>() // Roller için
    .AddEntityFrameworkStores<ApplicationDbContext>();

// MVC için gerekli servisleri ekle
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Hata sayfası ve HSTS ayarları
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS yönlendirmesi ve statik dosyaları kullan
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kimlik doğrulama ve yetkilendirme middleware'leri
app.UseAuthentication();
app.UseAuthorization();

// Varsayılan rota
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
