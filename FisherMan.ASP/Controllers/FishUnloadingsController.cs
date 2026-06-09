using FisherMan.ASP.Models;
using FisherMan.ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class FishUnloadingsController : Controller
    {
        private readonly FishermanContext _context;

        public FishUnloadingsController(FishermanContext context)
        {
            _context = context;
        }

        // GET: FishUnloadings
        public async Task<IActionResult> Index()
        {
            var unloadings = await _context.FishUnloadings
                .Include(f => f.LogbookEntry)
                    .ThenInclude(l => l!.Vessel)
                .OrderByDescending(f => f.UnloadingDate)
                .Take(100)
                .ToListAsync();

            return View(unloadings);
        }

        // GET: FishUnloadings/Create/5 (logbookEntryId)
        public async Task<IActionResult> Create(int logbookEntryId)
        {
            var entry = await _context.LogbookEntries
                .Include(l => l.Vessel)
                .FirstOrDefaultAsync(l => l.Id == logbookEntryId);

            if (entry == null) return NotFound();

            ViewBag.LogbookEntry = entry;
            return View(new FishUnloading { LogbookEntryId = logbookEntryId, QuantityKg = entry.CatchQuantityKg });
        }

        // POST: FishUnloadings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FishUnloading unloading)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unloading);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Fish unloading record created successfully.";
                return RedirectToAction("Index", "Logbook");
            }

            var entry = await _context.LogbookEntries.Include(l => l.Vessel).FirstOrDefaultAsync(l => l.Id == unloading.LogbookEntryId);
            ViewBag.LogbookEntry = entry;
            return View(unloading);
        }

        // GET: FishUnloadings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var unloading = await _context.FishUnloadings
                .Include(f => f.LogbookEntry)
                    .ThenInclude(l => l!.Vessel)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (unloading == null) return NotFound();

            ViewBag.LogbookEntry = unloading.LogbookEntry;
            return View(unloading);
        }

        // POST: FishUnloadings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FishUnloading unloading)
        {
            if (id != unloading.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unloading);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Fish unloading record updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnloadingExists(unloading.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(unloading);
        }

        // GET: FishUnloadings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var unloading = await _context.FishUnloadings
                .Include(f => f.LogbookEntry)
                    .ThenInclude(l => l!.Vessel)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (unloading == null) return NotFound();

            return View(unloading);
        }

        // POST: FishUnloadings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unloading = await _context.FishUnloadings.FindAsync(id);
            if (unloading != null)
            {
                _context.FishUnloadings.Remove(unloading);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Fish unloading record deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UnloadingExists(int id) => _context.FishUnloadings.Any(e => e.Id == id);
    }
}
