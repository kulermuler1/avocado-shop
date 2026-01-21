using System.ComponentModel.DataAnnotations;

namespace Avocado_shop.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        // Hierarchia kategorii - podkategorie
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? SubCategories { get; set; }

        // Dla kategorii cenowych (np. "do 3000 z³")
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Ikona kategorii (opcjonalnie)
        [StringLength(100)]
        public string? IconClass { get; set; }

        public ICollection<Product>? Products { get; set; }

        // Helper - czy to kategoria g³ówna
        public bool IsMainCategory => ParentCategoryId == null;
    }
}
