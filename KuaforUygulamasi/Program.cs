using KuaforUygulamasi.Data;
using KuaforUygulamasi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısını yapılandır
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDefaultIdentity<Kullanici>(options =>
{
    // Şifre gereksinimlerini daha esnek hale getirin
    options.Password.RequireDigit = false; // Rakam gereksinimini kaldır
    options.Password.RequireLowercase = false; // Küçük harf gereksinimini kaldır
    options.Password.RequireUppercase = false; // Büyük harf gereksinimini kaldır
    options.Password.RequiredLength = 3; // Şifrenin uzunluğunu 4 karakter olarak ayarla
    options.Password.RequireNonAlphanumeric = false; // Özel karakter gereksinimini kaldır
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

// Background service'i ekle
builder.Services.AddHostedService<RandevuOnayService>(); // RandevuOnayService'ini hosted service olarak kaydedin

var app = builder.Build();

// Hata sayfası ve HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var userManager = serviceProvider.GetRequiredService<UserManager<Kullanici>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Admin rolü oluştur
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        var roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
        if (!roleResult.Succeeded)
        {
            foreach (var error in roleResult.Errors)
            {
                Console.WriteLine($"Role Error: {error.Description}");
            }
            return;
        }
    }

    // İlk Admin kullanıcıyı oluştur
    var adminEmail1 = "G211210003@sakarya.edu.tr";
    var adminUser1 = await userManager.FindByEmailAsync(adminEmail1);
    if (adminUser1 == null)
    {
        adminUser1 = new Kullanici
        {
            UserName = adminEmail1,
            Email = adminEmail1,
            EmailConfirmed = true,
            Ad = "Admin",
            Soyad = "Kullanıcı",
            Rol = "Admin" // Rol değeri "Admin" olarak atanıyor
        };

        var createResult1 = await userManager.CreateAsync(adminUser1, "sau");
        if (!createResult1.Succeeded)
        {
            foreach (var error in createResult1.Errors)
            {
                Console.WriteLine($"Create User Error: {error.Description}");
            }
            return;
        }
    }

    // İlk Admin rolünü kullanıcıya ata
    if (!await userManager.IsInRoleAsync(adminUser1, "Admin"))
    {
        var roleAssignResult1 = await userManager.AddToRoleAsync(adminUser1, "Admin");
        if (!roleAssignResult1.Succeeded)
        {
            foreach (var error in roleAssignResult1.Errors)
            {
                Console.WriteLine($"Role Assign Error: {error.Description}");
            }
        }
    }

    // İkinci Admin kullanıcıyı oluştur
    var adminEmail2 = "G221210021@sakarya.edu.tr";
    var adminUser2 = await userManager.FindByEmailAsync(adminEmail2);
    if (adminUser2 == null)
    {
        adminUser2 = new Kullanici
        {
            UserName = adminEmail2,
            Email = adminEmail2,
            EmailConfirmed = true,
            Ad = "Admin",
            Soyad = "Kullanıcı2",
            Rol = "Admin" // Rol değeri "Admin" olarak atanıyor
        };

        var createResult2 = await userManager.CreateAsync(adminUser2, "sau");
        if (!createResult2.Succeeded)
        {
            foreach (var error in createResult2.Errors)
            {
                Console.WriteLine($"Create User Error: {error.Description}");
            }
            return;
        }
    }

    // İkinci Admin rolünü kullanıcıya ata
    if (!await userManager.IsInRoleAsync(adminUser2, "Admin"))
    {
        var roleAssignResult2 = await userManager.AddToRoleAsync(adminUser2, "Admin");
        if (!roleAssignResult2.Succeeded)
        {
            foreach (var error in roleAssignResult2.Errors)
            {
                Console.WriteLine($"Role Assign Error: {error.Description}");
            }
        }
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // Kimlik doğrulama
app.UseAuthorization();  // Yetkilendirme

// MVC Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "randevu",
    pattern: "Randevu/{action=Create}/{id?}",
    defaults: new { controller = "Randevu" });

app.MapControllerRoute(
    name: "calisan",
    pattern: "Calisan/{action=Create}/{id?}",
    defaults: new { controller = "Calisan" });

app.Run();
