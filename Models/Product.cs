using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avocado_shop.Models
{
    /// <summary>
    /// Model produktu w sklepie
    /// </summary>
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa produktu jest wymagana")]
        [StringLength(200, ErrorMessage = "Nazwa mo¿e mieæ maksymalnie 200 znaków")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; } = null!;

        [StringLength(2000, ErrorMessage = "Opis mo¿e mieæ maksymalnie 2000 znaków")]
        [Display(Name = "Opis")]
        public string? Description { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Cena musi byæ miêdzy 0 a 999999.99")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stan magazynowy nie mo¿e byæ ujemny")]
        [Display(Name = "Stan magazynowy")]
        public int Stock { get; set; }

        [StringLength(1000)]
        [Display(Name = "URL obrazu")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Kategoria")]
        public int CategoryId { get; set; }
        
        /// <summary>
        /// Kategoria, do której nale¿y produkt
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// Pozycje zamówieñ zawieraj¹ce ten produkt
        /// </summary>
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
