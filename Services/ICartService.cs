using Avocado_shop.Models;

namespace Avocado_shop.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(HttpContext httpContext, int productId, int quantity = 1);
        Task RemoveFromCartAsync(HttpContext httpContext, int productId);
        Task<IList<CartItem>> GetCartAsync(HttpContext httpContext);
        Task ClearCartAsync(HttpContext httpContext);
    }
}