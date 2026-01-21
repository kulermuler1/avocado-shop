using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Avocado_shop.Areas.Identity.Data;
using Avocado_shop.Models;

namespace Avocado_shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductAdminController : Controller
    {
        private readonly AvocadoDBContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductAdminController(AvocadoDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: ProductAdmin
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Category!.Name)
                .ThenBy(p => p.Name)
                .ToListAsync();
            return View(products);
        }

        // GET: ProductAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: ProductAdmin/Create
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View();
        }

        // POST: ProductAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Stock,CategoryId,ImageUrl")] Product product)
        {
            // Usuñ walidacjê dla Category (bo to navigation property)
            ModelState.Remove("Category");

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(product.CategoryId);
                return View(product);
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Produkt zosta³ dodany pomyœlnie.";
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            
            await LoadCategoriesAsync(product.CategoryId);
            return View(product);
        }

        // POST: ProductAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // Rêczne bindowanie z formularza
            product.Name = Request.Form["Name"].ToString();
            product.Description = Request.Form["Description"].ToString();
            product.Stock = int.TryParse(Request.Form["Stock"], out var stock) ? stock : 0;
            product.CategoryId = int.TryParse(Request.Form["CategoryId"], out var catId) ? catId : product.CategoryId;

            // Parsowanie ceny z obs³ug¹ kropki i przecinka
            var priceStr = Request.Form["Price"].ToString().Replace(',', '.');
            if (decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            {
                product.Price = price;
            }

            // ImageUrl z formularza (jeœli podano)
            var imageUrlFromForm = Request.Form["ImageUrl"].ToString();
            if (!string.IsNullOrWhiteSpace(imageUrlFromForm))
            {
                product.ImageUrl = imageUrlFromForm;
            }

            // Walidacja
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                ModelState.AddModelError("Name", "Nazwa jest wymagana.");
            }
            if (product.Price < 0)
            {
                ModelState.AddModelError("Price", "Cena nie mo¿e byæ ujemna.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(product.CategoryId);
                return View(product);
            }

            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Produkt zosta³ zaktualizowany pomyœlnie.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ProductAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            
            return View(product);
        }

        // POST: ProductAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Produkt zosta³ usuniêty.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helpers
        private bool ProductExists(int id) => _context.Products.Any(e => e.Id == id);

        private async Task LoadCategoriesAsync(int? selectedId = null)
        {
            var categories = await _context.Categories
                .Where(c => c.ParentCategoryId != null) // Tylko podkategorie
                .OrderBy(c => c.ParentCategory!.Name)
                .ThenBy(c => c.Name)
                .Select(c => new { c.Id, Name = c.ParentCategory!.Name + " > " + c.Name })
                .ToListAsync();

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", selectedId);
        }
    }
}
