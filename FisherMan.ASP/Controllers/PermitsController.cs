using FisherMan.ASP.Data;
using FisherMan.ASP.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class PermitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PermitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Permits
        public async Task<IActionResult> Index(string? search)
        {
            ViewData["Search"] = search;
            var permits = _context.FishingPermits.Include(p => p.Ship).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                permits = permits.Where(p =>
                    (p.Ship != null && p.Ship.OwnerName.Contains(search)) ||
                    (p.Ship != null && p.Ship.Marking.Contains(search)) ||
                    p.CaptainName.Contains(search));
            }
            return View(await permits.OrderByDescending(p => p.IssueDate).ToListAsync());
        }

        // GET: Permits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var permit = await _context.FishingPermits
                .Include(p => p.Ship)
                .Include(p => p.FishingTrips)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permit == null) return NotFound();

            return View(permit);
        }

        // GET: Permits/Create
        public IActionResult Create()
        {
            ViewData["ShipId"] = new SelectList(_context.Ships, "Id", "Marking");
            return View();
        }

        // POST: Permits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ShipId,IssueDate,ExpiryDate,IsRevoked,RevokedDate,CaptainName,FishingTools")] FishingPermit permit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShipId"] = new SelectList(_context.Ships, "Id", "Marking", permit.ShipId);
            return View(permit);
        }

        // GET: Permits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var permit = await _context.FishingPermits.FindAsync(id);
            if (permit == null) return NotFound();

            ViewData["ShipId"] = new SelectList(_context.Ships, "Id", "Marking", permit.ShipId);
            return View(permit);
        }

        // POST: Permits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ShipId,IssueDate,ExpiryDate,IsRevoked,RevokedDate,CaptainName,FishingTools")] FishingPermit permit)
        {
            if (id != permit.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.FishingPermits.Any(e => e.Id == permit.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShipId"] = new SelectList(_context.Ships, "Id", "Marking", permit.ShipId);
            return View(permit);
        }

        // GET: Permits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var permit = await _context.FishingPermits
                .Include(p => p.Ship)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permit == null) return NotFound();

            return View(permit);
        }

        // POST: Permits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permit = await _context.FishingPermits.FindAsync(id);
            if (permit != null) _context.FishingPermits.Remove(permit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
