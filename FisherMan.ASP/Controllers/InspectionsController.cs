using FisherMan.ASP.Data;
using FisherMan.ASP.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InspectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewData["Search"] = search;
            var inspections = _context.Inspections.Include(i => i.Inspector).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                inspections = inspections.Where(i => i.InspectedObject.Contains(search) ||
                    (i.Inspector != null && i.Inspector.Name.Contains(search)));
            return View(await inspections.OrderByDescending(i => i.InspectionDate).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var inspection = await _context.Inspections
                .Include(i => i.Inspector)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null) return NotFound();
            return View(inspection);
        }

        public IActionResult Create()
        {
            ViewData["InspectorId"] = new SelectList(_context.Inspectors, "Id", "Name");
            return View(new Inspection { InspectionDate = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InspectorId,InspectionDate,Type,InspectedObject,Notes,HasViolation,FineAmount,ActNumber")] Inspection inspection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InspectorId"] = new SelectList(_context.Inspectors, "Id", "Name", inspection.InspectorId);
            return View(inspection);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null) return NotFound();
            ViewData["InspectorId"] = new SelectList(_context.Inspectors, "Id", "Name", inspection.InspectorId);
            return View(inspection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InspectorId,InspectionDate,Type,InspectedObject,Notes,HasViolation,FineAmount,ActNumber")] Inspection inspection)
        {
            if (id != inspection.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Inspections.Any(e => e.Id == inspection.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["InspectorId"] = new SelectList(_context.Inspectors, "Id", "Name", inspection.InspectorId);
            return View(inspection);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var inspection = await _context.Inspections
                .Include(i => i.Inspector)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null) return NotFound();
            return View(inspection);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection != null) _context.Inspections.Remove(inspection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
