using System;
namespace KuaforUygulamasi.Models
{
    public class Randevu
    {
        public int Id { get; set; }
        public string MusteriAdi { get; set; }
        public DateTime Tarih { get; set; }
        public string HizmetTuru { get; set; } // Saç kesimi, boyama, bakım vb.
        public int MusteriId { get; internal set; }
    }
}

