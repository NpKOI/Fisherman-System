using FisherMan.ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class InspectionsController : Controller
    {
        private readonly FishermanContext _context;

        public InspectionsController(FishermanContext context)
        {
            _context = context;
        }

        // GET: Inspections
        public async Task<IActionResult> Index(bool? violationsOnly)
        {
            var query = _context.Inspections.Include(i => i.Vessel).AsQueryable();

            if (violationsOnly == true)
                query = query.Where(i => i.ViolationFound);

            ViewBag.ViolationsOnly = violationsOnly;
            return View(await query.OrderByDescending(i => i.InspectionDate).ToListAsync());
        }

        // GET: Inspections/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Vessels = new SelectList(await _context.Vessels.ToListAsync(), "Id", "InternationalNumber");
            return View();
        }

        // POST: Inspections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inspection inspection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inspection);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Inspection recorded.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Vessels = new SelectList(await _context.Vessels.ToListAsync(), "Id", "InternationalNumber", inspection.VesselId);
            return View(inspection);
        }

        // GET: Inspections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null) return NotFound();

            ViewBag.Vessels = new SelectList(await _context.Vessels.ToListAsync(), "Id", "InternationalNumber", inspection.VesselId);
            return View(inspection);
        }

        // POST: Inspections/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inspection inspection)
        {
            if (id != inspection.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(inspection);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Inspection updated.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Vessels = new SelectList(await _context.Vessels.ToListAsync(), "Id", "InternationalNumber", inspection.VesselId);
            return View(inspection);
        }

        // GET: Inspections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var inspection = await _context.Inspections.Include(i => i.Vessel).FirstOrDefaultAsync(i => i.Id == id);
            if (inspection == null) return NotFound();
            return View(inspection);
        }

        // POST: Inspections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection != null)
            {
                _context.Inspections.Remove(inspection);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Inspection deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
