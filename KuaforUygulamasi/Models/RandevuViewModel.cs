using System.ComponentModel.DataAnnotations;

namespace KuaforUygulamasi.Models
{
    public class RandevuViewModel
    {
        public int ID { get; set; }

        [Required]
        public string KullaniciID { get; set; }
        public Kullanici Kullanici { get; set; }

        [Required]
        public int CalisanID { get; set; }
        public Calisan Calisan { get; set; }

        [Required]
        public int IslemID { get; set; }
        public Islem Islem { get; set; }

        [Required]
        public DateTime Saat { get; set; }
        public string Durum { get; set; }

        public List<Kullanici> KullaniciListesi { get; set; }
        public List<Calisan> CalisanListesi { get; set; }
        public List<Islem> IslemListesi { get; set; }
    }
}
