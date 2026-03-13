using FisherMan.ASP.Data;
using FisherMan.ASP.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class ShipsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShipsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ships
        public async Task<IActionResult> Index(string? search)
        {
            ViewData["Search"] = search;
            var ships = _context.Ships.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                ships = ships.Where(s =>
                    s.InternationalNumber.Contains(search) ||
                    s.Marking.Contains(search) ||
                    s.OwnerName.Contains(search) ||
                    s.CaptainName.Contains(search));
            }
            return View(await ships.OrderBy(s => s.OwnerName).ToListAsync());
        }

        // GET: Ships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ship = await _context.Ships
                .Include(s => s.Permits)
                .ThenInclude(p => p.FishingTrips)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ship == null) return NotFound();

            return View(ship);
        }

        // GET: Ships/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ships/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InternationalNumber,CallSign,Marking,OwnerName,OwnerEGN,CaptainName,Length,Width,Tonnage,Draft,EnginePower,EngineType,FuelType,FuelConsumptionPerHour")] Ship ship)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ship);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ship);
        }

        // GET: Ships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ship = await _context.Ships.FindAsync(id);
            if (ship == null) return NotFound();
            return View(ship);
        }

        // POST: Ships/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InternationalNumber,CallSign,Marking,OwnerName,OwnerEGN,CaptainName,Length,Width,Tonnage,Draft,EnginePower,EngineType,FuelType,FuelConsumptionPerHour")] Ship ship)
        {
            if (id != ship.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ship);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipExists(ship.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ship);
        }

        // GET: Ships/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ship = await _context.Ships.FirstOrDefaultAsync(m => m.Id == id);
            if (ship == null) return NotFound();

            return View(ship);
        }

        // POST: Ships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ship = await _context.Ships.FindAsync(id);
            if (ship != null) _context.Ships.Remove(ship);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipExists(int id)
        {
            return _context.Ships.Any(e => e.Id == id);
        }
    }
}
