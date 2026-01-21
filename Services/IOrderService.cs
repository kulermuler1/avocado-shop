using Avocado_shop.Models;

namespace Avocado_shop.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string userId, IEnumerable<CartItem> items, Avocado_shop.Areas.Identity.Data.AvocadoDBContext db);
    }
}