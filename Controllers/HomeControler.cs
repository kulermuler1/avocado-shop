using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Avocado_shop.Areas.Identity.Data;

namespace Avocado_shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AvocadoDBContext _db;

        public HomeController(AvocadoDBContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            // Pobierz główne kategorie (bez rodzica)
            var mainCategories = await _db.Categories
                .Where(c => c.ParentCategoryId == null)
                .OrderBy(c => c.Name)
                .ToListAsync();

            // Pobierz polecane produkty (np. losowe 8)
            var availableProducts = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.Stock > 0)
                .ToListAsync();

            var featuredProducts = availableProducts
                .OrderBy(_ => Random.Shared.Next())
                .Take(8)
                .ToList();

            ViewData["MainCategories"] = mainCategories;
            ViewData["FeaturedProducts"] = featuredProducts;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}