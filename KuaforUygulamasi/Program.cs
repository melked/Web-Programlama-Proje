using KuaforUygulamasi.Data;
using KuaforUygulamasi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısını yapılandır
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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

    // Admin kullanıcı oluştur
    var adminEmail = "OgrenciNuramarasi@sakarya.edu.tr";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new Kullanici
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Ad = "Admin",
            Soyad = "Kullanıcı",
            Rol = "Admin" // Rol değeri "Admin" olarak atanıyor
        };

        var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                Console.WriteLine($"Create User Error: {error.Description}");
            }
            return;
        }
    }

    // Admin rolünü kullanıcıya ata
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        var roleAssignResult = await userManager.AddToRoleAsync(adminUser, "Admin");
        if (!roleAssignResult.Succeeded)
        {
            foreach (var error in roleAssignResult.Errors)
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

app.MapControllerRoute(
    name: "islem",
    pattern: "Islem/{action=Index}/{id?}",
    defaults: new { controller = "Islem" });

app.Run();
