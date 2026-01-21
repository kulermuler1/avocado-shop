using Avocado_shop.Models;
using System.Text.Json;

namespace Avocado_shop.Services
{
    public class CartService : ICartService
    {
        private const string SessionKey = "CartSession";

        public async Task AddToCartAsync(HttpContext httpContext, int productId, int quantity = 1)
        {
            var cart = await GetCartAsync(httpContext);
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                cart.Add(new CartItem { ProductId = productId, Quantity = quantity });
            }
            else
            {
                item.Quantity += quantity;
            }
            SaveSession(httpContext, cart);
        }

        public async Task RemoveFromCartAsync(HttpContext httpContext, int productId)
        {
            var cart = await GetCartAsync(httpContext);
            var item = cart.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveSession(httpContext, cart);
            }
        }

        public Task<IList<CartItem>> GetCartAsync(HttpContext httpContext)
        {
            var json = httpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json))
            {
                return Task.FromResult((IList<CartItem>)new List<CartItem>());
            }
            var cart = JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
            return Task.FromResult((IList<CartItem>)cart);
        }

        public Task ClearCartAsync(HttpContext httpContext)
        {
            httpContext.Session.Remove(SessionKey);
            return Task.CompletedTask;
        }

        private void SaveSession(HttpContext httpContext, IList<CartItem> cart)
        {
            var json = JsonSerializer.Serialize(cart);
            httpContext.Session.SetString(SessionKey, json);
        }
    }
}