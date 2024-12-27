using KuaforUygulamasi.Models;
using System.ComponentModel.DataAnnotations;
namespace KuaforUygulamasi.Models
{
    public class Randevu
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Müşteri seçimi zorunludur")]
        public string KullaniciId { get; set; }

        public virtual Kullanici? Kullanici { get; set; }

        [Required(ErrorMessage = "Çalışan seçimi zorunludur")]
        public int CalisanID { get; set; }

        public virtual Calisan? Calisan { get; set; }

        [Required(ErrorMessage = "İşlem seçimi zorunludur")]
        public int IslemID { get; set; }

        public virtual Islem? Islem { get; set; }

        [Required(ErrorMessage = "Randevu saati zorunludur")]
        public DateTimeOffset Saat { get; set; } // DateTime yerine DateTimeOffset kullanalım

        [Required(ErrorMessage = "Durum alanı zorunludur")]
        [StringLength(50)]
        public string Durum { get; set; } = "Beklemede";
    }
}