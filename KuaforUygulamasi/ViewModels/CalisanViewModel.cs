using KuaforUygulamasi.Models;
using System.ComponentModel.DataAnnotations;

namespace KuaforUygulamasi.ViewModels
{
    public class CalisanViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = "En az bir uzmanlık alanı seçilmelidir.")]
        [Display(Name = "Uzmanlık Alanları")]
        public List<UzmanlikAlani> SelectedUzmanlikAlanlari { get; set; } = new List<UzmanlikAlani>();

        [Required(ErrorMessage = "Müsaitlik başlangıç saati zorunludur.")]
        [DataType(DataType.Time)]
        [Display(Name = "Mesai Başlangıç")]
        public TimeSpan MusaitlikBaslangic { get; set; }

        [Required(ErrorMessage = "Müsaitlik bitiş saati zorunludur.")]
        [DataType(DataType.Time)]
        [Display(Name = "Mesai Bitiş")]
        public TimeSpan MusaitlikBitis { get; set; }

        public List<UzmanlikAlani> TumUzmanlikAlanlari => Enum.GetValues<UzmanlikAlani>().ToList();
    }
}