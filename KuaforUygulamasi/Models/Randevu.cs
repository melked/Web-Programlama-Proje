using System.ComponentModel.DataAnnotations;

namespace KuaforUygulamasi.Models
{
    public class Randevu
    {
        public int ID { get; set; }
        [Required]
        public int KullaniciID { get; set; } // Foreign Key
        public Kullanici Kullanici { get; set; } // Navigation Property
        [Required]
        public int CalisanID { get; set; } // Foreign Key
        public Calisan Calisan { get; set; } // Navigation Property
        [Required]
        public int IslemID { get; set; } // Foreign Key
        public Islem Islem { get; set; } // Navigation Property
        [Required]
        public DateTime Saat { get; set; }
        public string Durum { get; set; } // "Onaylandı" veya "Beklemede"
    }
}
