namespace KuaforUygulamasi.Models
{
    public class RandevuViewModel
    {
        public int ID { get; set; }
        public int KullaniciID { get; set; } // Seçilen Kullanıcı ID
        public Kullanici Kullanici { get; set; } // Kullanıcı Bilgileri
        public int CalisanID { get; set; } // Seçilen Çalışan ID
        public Calisan Calisan { get; set; } // Çalışan Bilgileri
        public int IslemID { get; set; } // Seçilen İşlem ID
        public Islem Islem { get; set; } // İşlem Bilgileri
        public DateTime Saat { get; set; } // Randevu Saati
        public string Durum { get; set; } // "Onaylandı" veya "Beklemede"

        // Kullanıcı Seçimi için Liste
        public List<Kullanici> KullaniciListesi { get; set; }
        // Çalışan Seçimi için Liste
        public List<Calisan> CalisanListesi { get; set; }
        // İşlem Seçimi için Liste
        public List<Islem> IslemListesi { get; set; }
    }
}
