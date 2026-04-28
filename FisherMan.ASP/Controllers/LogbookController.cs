using FisherMan.ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class LogbookController : Controller
    {
        private readonly FishermanContext _context;

        public LogbookController(FishermanContext context)
        {
            _context = context;
        }

        // GET: Logbook
        public async Task<IActionResult> Index(int? vesselId, DateTime? from, DateTime? to)
        {
            var query = _context.LogbookEntries
                .Include(l => l.Vessel)
                .Include(l => l.Permit)
                .Include(l => l.Unloadings)
                .AsQueryable();

            if (vesselId.HasValue)
                query = query.Where(l => l.VesselId == vesselId.Value);
            if (from.HasValue)
                query = query.Where(l => l.StartTime >= from.Value);
            if (to.HasValue)
                query = query.Where(l => l.StartTime <= to.Value);

            ViewBag.Vessels = new SelectList(await _context.Vessels.ToListAsync(), "Id", "InternationalNumber", vesselId);
            ViewBag.VesselId = vesselId;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            return View(await query.OrderByDescending(l => l.StartTime).ToListAsync());
        }

        // GET: Logbook/Create
        public async Task<IActionResult> Create()
        {
            // Only vessels over 10m require logbook entries
            ViewBag.Vessels = new SelectList(
                await _context.Vessels.Where(v => v.Length > 10).ToListAsync(),
                "Id", "InternationalNumber");
            ViewBag.Permits = new SelectList(
                await _context.Permits.Where(p => !p.IsRevoked && p.ExpiryDate >= DateTime.Today)
                    .Include(p => p.Vessel).ToListAsync(),
                "Id", "Vessel.InternationalNumber");
            return View();
        }

        // POST: Logbook/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LogbookEntry entry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entry);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Logbook entry recorded.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Vessels = new SelectList(
                await _context.Vessels.Where(v => v.Length > 10).ToListAsync(),
                "Id", "InternationalNumber", entry.VesselId);
            ViewBag.Permits = new SelectList(
                await _context.Permits.Where(p => !p.IsRevoked && p.ExpiryDate >= DateTime.Today)
                    .Include(p => p.Vessel).ToListAsync(),
                "Id", "Vessel.InternationalNumber", entry.PermitId);
            return View(entry);
        }

        // GET: Logbook/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.LogbookEntries.FindAsync(id);
            if (entry == null) return NotFound();

            ViewBag.Vessels = new SelectList(
                await _context.Vessels.Where(v => v.Length > 10).ToListAsync(),
                "Id", "InternationalNumber", entry.VesselId);
            ViewBag.Permits = new SelectList(
                await _context.Permits.Include(p => p.Vessel).ToListAsync(),
                "Id", "Vessel.InternationalNumber", entry.PermitId);
            return View(entry);
        }

        // POST: Logbook/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LogbookEntry entry)
        {
            if (id != entry.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(entry);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Logbook entry updated.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Vessels = new SelectList(
                await _context.Vessels.Where(v => v.Length > 10).ToListAsync(),
                "Id", "InternationalNumber", entry.VesselId);
            ViewBag.Permits = new SelectList(
                await _context.Permits.Include(p => p.Vessel).ToListAsync(),
                "Id", "Vessel.InternationalNumber", entry.PermitId);
            return View(entry);
        }

        // GET: Logbook/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var entry = await _context.LogbookEntries.Include(l => l.Vessel).FirstOrDefaultAsync(l => l.Id == id);
            if (entry == null) return NotFound();
            return View(entry);
        }

        // POST: Logbook/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry = await _context.LogbookEntries.FindAsync(id);
            if (entry != null)
            {
                _context.LogbookEntries.Remove(entry);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Logbook entry deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
