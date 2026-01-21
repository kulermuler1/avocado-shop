using Avocado_shop.Areas.Identity.Data;
using Avocado_shop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avocado_shop.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ICartService _cart;
        private readonly IOrderService _orderService;
        private readonly AvocadoDBContext _db;
        private readonly UserManager<IdentityUser> _um;

        public OrdersController(ICartService cart, IOrderService orderService, AvocadoDBContext db, UserManager<IdentityUser> um)
        {
            _cart = cart;
            _orderService = orderService;
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _um.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var orders = await _db.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Checkout()
        {
            var items = await _cart.GetCartAsync(HttpContext);
            if (!items.Any()) return RedirectToAction("Index", "Cart");
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var user = await _um.GetUserAsync(User);
            var items = await _cart.GetCartAsync(HttpContext);
            var order = await _orderService.CreateOrderAsync(user?.Id ?? "", items, _db);
            await _cart.ClearCartAsync(HttpContext);
            return RedirectToAction("Details", new { id = order.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders.Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}