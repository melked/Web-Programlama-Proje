using System.ComponentModel.DataAnnotations;

namespace KuaforUygulamasi.Models
{
    public class Calisan
    {
        public int ID { get; set; }
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string Soyad { get; set; }

        public string UzmanlikAlanlari { get; set; }
        public string MusaitlikSaatleri { get; set; }
    }
}
