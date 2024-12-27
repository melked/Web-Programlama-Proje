using KuaforUygulamasi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<Kullanici>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Randevu> Randevular { get; set; }
    public DbSet<Calisan> Calisanlar { get; set; }
    public DbSet<Islem> Islemler { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure relationships
        builder.Entity<Randevu>()
            .HasOne(r => r.Kullanici)
            .WithMany()
            .HasForeignKey(r => r.KullaniciId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Randevu>()
            .HasOne(r => r.Calisan)
            .WithMany()
            .HasForeignKey(r => r.CalisanID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Randevu>()
            .HasOne(r => r.Islem)
            .WithMany()
            .HasForeignKey(r => r.IslemID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
