using FisherMan.ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class VesselsController : Controller
    {
        private readonly FishermanContext _context;

        public VesselsController(FishermanContext context)
        {
            _context = context;
        }

        // GET: Vessels
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Vessels.Include(v => v.Owner).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(v =>
                    v.InternationalNumber.Contains(search) ||
                    v.Owner!.FullName.Contains(search) ||
                    v.CaptainName.Contains(search) ||
                    v.Marking.Contains(search));
            }

            return View(await query.OrderBy(v => v.InternationalNumber).ToListAsync());
        }

        // GET: Vessels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var vessel = await _context.Vessels
                .Include(v => v.Owner)
                .Include(v => v.Permits).ThenInclude(p => p.Person)
                .Include(v => v.LogbookEntries.OrderByDescending(l => l.StartTime).Take(10))
                .Include(v => v.Inspections.OrderByDescending(i => i.InspectionDate).Take(5))
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vessel == null) return NotFound();

            return View(vessel);
        }

        // GET: Vessels/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Owners = await _context.Persons.OrderBy(p => p.FullName).ToListAsync();
            return View();
        }

        // POST: Vessels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vessel vessel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vessel);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Vessel created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Owners = await _context.Persons.OrderBy(p => p.FullName).ToListAsync();
            return View(vessel);
        }

        // GET: Vessels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var vessel = await _context.Vessels.FindAsync(id);
            if (vessel == null) return NotFound();

            ViewBag.Owners = await _context.Persons.OrderBy(p => p.FullName).ToListAsync();
            return View(vessel);
        }

        // POST: Vessels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vessel vessel)
        {
            if (id != vessel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vessel);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Vessel updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VesselExists(vessel.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Owners = await _context.Persons.OrderBy(p => p.FullName).ToListAsync();
            return View(vessel);
        }

        // GET: Vessels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var vessel = await _context.Vessels
                .Include(v => v.Owner)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (vessel == null) return NotFound();

            return View(vessel);
        }

        // POST: Vessels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vessel = await _context.Vessels.FindAsync(id);
            if (vessel != null)
            {
                _context.Vessels.Remove(vessel);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Vessel deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VesselExists(int id) => _context.Vessels.Any(e => e.Id == id);
    }
}
