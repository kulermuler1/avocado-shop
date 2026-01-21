using Avocado_shop.Areas.Identity.Data;
using Avocado_shop.Models;

namespace Avocado_shop.Services
{
    public class OrderService : IOrderService
    {
        public async Task<Order> CreateOrderAsync(string userId, IEnumerable<CartItem> items, AvocadoDBContext db)
        {
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Nowe",
                TotalPrice = 0m,
                Items = new List<OrderItem>()
            };

            foreach (var ci in items)
            {
                var product = await db.Products.FindAsync(ci.ProductId);
                if (product == null) continue;
                
                var price = product.Price;
                order.Items!.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = ci.Quantity,
                    Price = price
                });
                order.TotalPrice += price * ci.Quantity;
                product.Stock = Math.Max(0, product.Stock - ci.Quantity);
            }

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            return order;
        }
    }
}
