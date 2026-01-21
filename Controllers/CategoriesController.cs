using Avocado_shop.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avocado_shop.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AvocadoDBContext _db;

        public CategoriesController(AvocadoDBContext db) => _db = db;

        // GET: /Categories - lista g³ównych kategorii
        public async Task<IActionResult> Index()
        {
            var mainCategories = await _db.Categories
                .Where(c => c.ParentCategoryId == null)
                .Include(c => c.SubCategories)
                .ToListAsync();

            return View(mainCategories);
        }

        // GET: /Categories/5 - podkategorie i produkty danej kategorii
        public async Task<IActionResult> Details(int id)
        {
            var category = await _db.Categories
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            // Jeœli kategoria ma podkategorie, poka¿ je
            // Jeœli nie ma podkategorii, poka¿ produkty
            return View(category);
        }

        // GET: /Categories/5/Products - produkty w kategorii z filtrowaniem
        public async Task<IActionResult> Products(int id, string? sort, decimal? minPrice, decimal? maxPrice)
        {
            var category = await _db.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            // Pobierz produkty z tej kategorii i jej podkategorii
            var categoryIds = new List<int> { id };

            // Dodaj ID podkategorii
            var subCategoryIds = await _db.Categories
                .Where(c => c.ParentCategoryId == id)
                .Select(c => c.Id)
                .ToListAsync();
            categoryIds.AddRange(subCategoryIds);

            var productsQuery = _db.Products
                .Include(p => p.Category)
                .Where(p => categoryIds.Contains(p.CategoryId));

            // Filtrowanie po cenie
            if (minPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);

            // Jeœli kategoria ma progi cenowe, zastosuj je
            if (category.MinPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price >= category.MinPrice.Value);
            if (category.MaxPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price <= category.MaxPrice.Value);

            // Sortowanie
            productsQuery = sort switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                "name" => productsQuery.OrderBy(p => p.Name),
                _ => productsQuery.OrderBy(p => p.Name)
            };

            var products = await productsQuery.ToListAsync();

            ViewBag.Category = category;
            ViewBag.Sort = sort;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(products);
        }
    }
}
