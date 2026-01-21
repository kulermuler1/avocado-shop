using Avocado_shop.Areas.Identity.Data;
using Avocado_shop.Services;
using Microsoft.AspNetCore.Mvc;

namespace Avocado_shop.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cart;
        private readonly AvocadoDBContext _db;

        public CartController(ICartService cart, AvocadoDBContext db)
        {
            _cart = cart;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _cart.GetCartAsync(HttpContext);
            var products = new List<(Models.Product product, int qty)>();
            foreach (var it in items)
            {
                var p = await _db.Products.FindAsync(it.ProductId);
                if (p != null) products.Add((p, it.Quantity));
            }
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, int qty = 1)
        {
            await _cart.AddToCartAsync(HttpContext, productId, qty);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            await _cart.RemoveFromCartAsync(HttpContext, productId);
            return RedirectToAction("Index");
        }
    }
}