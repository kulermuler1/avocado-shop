using Avocado_shop.Areas.Identity.Data;
using Avocado_shop.Models;
using Avocado_shop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Avocado_shop.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cart;
        private readonly IOrderService _orderService;
        private readonly AvocadoDBContext _db;
        private readonly UserManager<IdentityUser> _um;

        public CheckoutController(ICartService cart, IOrderService orderService, AvocadoDBContext db, UserManager<IdentityUser> um)
        {
            _cart = cart;
            _orderService = orderService;
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _cart.GetCartAsync(HttpContext);
            if (!items.Any()) return RedirectToAction("Index", "Cart");
            return View(new CheckoutViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Place(CheckoutViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);

            var user = await _um.GetUserAsync(User);
            var items = await _cart.GetCartAsync(HttpContext);

            // payment stub: tutaj wstaw integracjê z payment gateway (obecnie symulacja)
            // jeœli payment ok -> create order
            var order = await _orderService.CreateOrderAsync(user?.Id ?? "", items, _db);

            // opcjonalnie uzupe³nij adres w zamówieniu (mo¿esz dodaæ pola w Order)
            // Wyœlij mail potwierdzaj¹cy (orderservice robi to minimalnie)

            await _cart.ClearCartAsync(HttpContext);
            return RedirectToAction("Details", "Orders", new { id = order.Id });
        }
    }
}