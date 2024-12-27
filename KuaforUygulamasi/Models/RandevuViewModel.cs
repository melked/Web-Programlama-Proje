using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace KuaforUygulamasi.Models
{
    public class RandevuViewModel
    {
        [Required(ErrorMessage = "Kullanıcı seçimi zorunludur")]
        public string KullaniciID { get; set; }

        [Required(ErrorMessage = "Çalışan seçimi zorunludur")]
        public int CalisanID { get; set; }

        [Required(ErrorMessage = "İşlem seçimi zorunludur")]
        public int IslemID { get; set; }

        [Required(ErrorMessage = "Randevu saati zorunludur")]
        public DateTime Saat { get; set; }

        public List<Kullanici> KullaniciListesi { get; set; }
        public List<Calisan> CalisanListesi { get; set; }
        public List<Islem> IslemListesi { get; set; }
    }
}
