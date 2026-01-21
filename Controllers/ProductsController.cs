using Avocado_shop.Areas.Identity.Data;
using Avocado_shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avocado_shop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AvocadoDBContext _db;

        public ProductsController(AvocadoDBContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}