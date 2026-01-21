using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Avocado_shop.Areas.Identity.Data;
using Avocado_shop.Models;

namespace Avocado_shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryAdminController : Controller
    {
        private readonly AvocadoDBContext _context;
        public CategoryAdminController(AvocadoDBContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .OrderBy(c => c.ParentCategoryId)
                .ThenBy(c => c.Name)
                .ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateParentCategoriesDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentCategoriesDropdown(category.ParentCategoryId);
                return View(category);
            }
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            await PopulateParentCategoriesDropdown(category.ParentCategoryId, category.Id);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                await PopulateParentCategoriesDropdown(category.ParentCategoryId, category.Id);
                return View(category);
            }
            _context.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateParentCategoriesDropdown(int? selectedId = null, int? excludeId = null)
        {
            var categories = await _context.Categories
                .Where(c => c.ParentCategoryId == null && c.Id != excludeId)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewData["ParentCategoryId"] = new SelectList(categories, "Id", "Name", selectedId);
        }
    }
}