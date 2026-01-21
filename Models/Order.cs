using System.ComponentModel.DataAnnotations;

namespace Avocado_shop.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Status { get; set; } = "New";

        [Range(0, 999999.99)]
        public decimal TotalPrice { get; set; }

        public string? UserId { get; set; }

        public ICollection<OrderItem>? Items { get; set; }
    }
}
