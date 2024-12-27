using System.ComponentModel.DataAnnotations;

namespace KuaforUygulamasi.Models
{
    public class RandevuViewModel
    {
        public int ID { get; set; }

        [Required]
        public string KullaniciID { get; set; }
        public Kullanici Kullanici { get; set; } // Kullanıcı bilgileri

        [Required]
        public int CalisanID { get; set; }
        public Calisan Calisan { get; set; } // Çalışan bilgileri

        [Required]
        public int IslemID { get; set; }
        public Islem Islem { get; set; } // İşlem bilgileri

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Saat { get; set; } // Randevu saati

        [StringLength(50)]
        public string Durum { get; set; } // Randevu durumu ("Beklemede", "Onaylandı")

        // Kullanıcı, çalışan ve işlem listelerini doldurmak için
        public List<Kullanici> KullaniciListesi { get; set; }
        public List<Calisan> CalisanListesi { get; set; }
        public List<Islem> IslemListesi { get; set; }
    }
}
