using System.ComponentModel.DataAnnotations.Schema;

namespace KuaforUygulamasi.Models
{
    public class Islem
    {
        public int ID { get; set; }
        public string Ad { get; set; }
        public int Sure { get; set; } // Dakika cinsinden
        [Column(TypeName = "decimal(18,2)")]
        public decimal Ucret { get; set; }

        // Uzmanlık Alanı özelliği ekleniyor
        public UzmanlikAlani UzmanlikAlani { get; set; }
    }

}
