using System.ComponentModel.DataAnnotations;

namespace Avocado_shop.Models
{
    public class CheckoutViewModel
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required, StringLength(200)]
        public string AddressLine { get; set; } = null!;

        [Required, StringLength(100)]
        public string City { get; set; } = null!;

        [Required, StringLength(20)]
        public string PostalCode { get; set; } = null!;

        [Required, StringLength(100)]
        public string Country { get; set; } = null!;
    }
}