using System.ComponentModel.DataAnnotations;

namespace KuaforUygulamasi.Models
{
    public class Calisan
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string Soyad { get; set; }

        public string UzmanlikAlanlari { get; set; }

        [Required(ErrorMessage = "Müsaitlik başlangıç saati zorunludur.")]
        [DataType(DataType.Time)]
        public TimeSpan MusaitlikBaslangic { get; set; } // Çalışanın müsaitlik başlangıç saati

        [Required(ErrorMessage = "Müsaitlik bitiş saati zorunludur.")]
        [DataType(DataType.Time)]
        public TimeSpan MusaitlikBitis { get; set; } // Çalışanın müsaitlik bitiş saati
    }
}
