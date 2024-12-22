using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KuaforUygulamasi.Models;
using System.Collections.Generic;

namespace KuaforUygulamasi.Data
{
    public class ApplicationDbContext : IdentityDbContext <Kullanici>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tabloları temsil eden DbSet'leri ekleyin.
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
        public DbSet<Islem> Islemler { get; set; }
        public DbSet<Calisan> Calisanlar { get; set; }
    }
}
