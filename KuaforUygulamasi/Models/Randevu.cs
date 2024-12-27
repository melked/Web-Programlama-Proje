using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KuaforUygulamasi.Models
{
    public class Randevu
    {
        public int ID { get; set; }

        [Required]
        public string KullaniciID { get; set; } // Foreign Key
        [ForeignKey("KullaniciID")]
        public Kullanici Kullanici { get; set; } // Navigation Property

        [Required]
        public int CalisanID { get; set; } // Foreign Key
        [ForeignKey("CalisanID")]
        public Calisan Calisan { get; set; } // Navigation Property

        [Required]
        public int IslemID { get; set; } // Foreign Key
        [ForeignKey("IslemID")]
        public Islem Islem { get; set; } // Navigation Property

        [Required]
        public DateTime Saat { get; set; } // Randevu saati

        [Required]
        [StringLength(50)]
        public string Durum { get; set; } // Örn: "Onaylandı" veya "Beklemede"
    }
}
