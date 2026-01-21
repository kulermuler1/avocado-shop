using Avocado_shop.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avocado_shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AvocadoDBContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(AvocadoDBContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /Admin - Dashboard
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalOrders = await _db.Orders.CountAsync();
            ViewBag.TotalProducts = await _db.Products.CountAsync();
            ViewBag.TotalCategories = await _db.Categories.CountAsync();
            ViewBag.TotalRevenue = await _db.Orders.SumAsync(o => o.TotalPrice);
            ViewBag.NewOrders = await _db.Orders.CountAsync(o => o.Status == "Nowe");
            
            // Ostatnie zamówienia
            ViewBag.RecentOrders = await _db.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            return View();
        }

        // GET: /Admin/Users - Lista u¿ytkowników
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles.ToList(),
                    // LockoutEnd mo¿na u¿yæ jako przybli¿on¹ datê rejestracji (lub dodaæ pole do modelu)
                    RegistrationDate = user.LockoutEnd?.DateTime
                });
            }

            return View(userList);
        }

        // GET: /Admin/Orders - Lista wszystkich zamówieñ
        public async Task<IActionResult> Orders(string? status)
        {
            var query = _db.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // Pobierz emaile u¿ytkowników
            var userIds = orders.Select(o => o.UserId).Distinct().ToList();
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);

            ViewBag.UserEmails = users;
            ViewBag.CurrentStatus = status;

            return View(orders);
        }

        // POST: /Admin/Orders/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Orders));
        }

        // POST: /Admin/Users/ToggleRole
        [HttpPost]
        public async Task<IActionResult> ToggleAdminRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
            return RedirectToAction(nameof(Users));
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserName { get; set; } = "";
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime? RegistrationDate { get; set; }
    }
}
