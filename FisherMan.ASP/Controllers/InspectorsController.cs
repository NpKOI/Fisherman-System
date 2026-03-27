using FisherMan.ASP.Data;
using FisherMan.ASP.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class InspectorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InspectorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewData["Search"] = search;
            var inspectors = _context.Inspectors.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                inspectors = inspectors.Where(i => i.Name.Contains(search) || i.BadgeNumber.Contains(search));
            return View(await inspectors.OrderBy(i => i.Name).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var inspector = await _context.Inspectors
                .Include(i => i.Inspections)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspector == null) return NotFound();
            return View(inspector);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,BadgeNumber,Phone")] Inspector inspector)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inspector);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var inspector = await _context.Inspectors.FindAsync(id);
            if (inspector == null) return NotFound();
            return View(inspector);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BadgeNumber,Phone")] Inspector inspector)
        {
            if (id != inspector.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspector);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Inspectors.Any(e => e.Id == inspector.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(inspector);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var inspector = await _context.Inspectors.FirstOrDefaultAsync(m => m.Id == id);
            if (inspector == null) return NotFound();
            return View(inspector);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspector = await _context.Inspectors.FindAsync(id);
            if (inspector != null) _context.Inspectors.Remove(inspector);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
