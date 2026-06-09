using FisherMan.ASP.Models;
using FisherMan.ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FisherMan.ASP.Controllers
{
    public class PermitsController : Controller
    {
        private readonly FishermanContext _context;

        public PermitsController(FishermanContext context)
        {
            _context = context;
        }

        // GET: Permits
        public async Task<IActionResult> Index(string? status)
        {
            var today = DateTime.Today;
            var query = _context.Permits.Include(p => p.Vessel).Include(p => p.Person).AsQueryable();

            query = status switch
            {
                "active" => query.Where(p => !p.IsRevoked && p.ExpiryDate >= today),
                "expired" => query.Where(p => p.ExpiryDate < today),
                "revoked" => query.Where(p => p.IsRevoked),
                _ => query
            };

            ViewBag.Status = status;
            return View(await query.OrderByDescending(p => p.IssuedDate).ToListAsync());
        }

        // GET: Permits/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Vessels = new SelectList(await _context.Vessels.ToListAsync(), "Id", "InternationalNumber");
            ViewBag.Persons = new SelectList(await _context.Persons.ToListAsync(), "Id", "FullName");
            return View();
        }

        // POST: Permits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Permit permit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Permit issued successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Vessels = new SelectList(await _context.Vessels.ToListAsync(), "Id", "InternationalNumber", permit.VesselId);
            ViewBag.Persons = new SelectList(await _context.Persons.ToListAsync(), "Id", "FullName", permit.PersonId);
            return View(permit);
        }

        // GET: Permits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var permit = await _context.Permits.FindAsync(id);
            if (permit == null) return NotFound();

            ViewBag.Vessels = new SelectList(await _context.Vessels.ToListAsync(), "Id", "InternationalNumber", permit.VesselId);
            ViewBag.Persons = new SelectList(await _context.Persons.ToListAsync(), "Id", "FullName", permit.PersonId);
            return View(permit);
        }

        // POST: Permits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Permit permit)
        {
            if (id != permit.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(permit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Permit updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Vessels = new SelectList(await _context.Vessels.ToListAsync(), "Id", "InternationalNumber", permit.VesselId);
            ViewBag.Persons = new SelectList(await _context.Persons.ToListAsync(), "Id", "FullName", permit.PersonId);
            return View(permit);
        }

        // POST: Permits/Revoke/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(int id, string reason)
        {
            var permit = await _context.Permits.FindAsync(id);
            if (permit == null) return NotFound();

            permit.IsRevoked = true;
            permit.RevocationReason = reason;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Permit revoked.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Permits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var permit = await _context.Permits.Include(p => p.Vessel).FirstOrDefaultAsync(p => p.Id == id);
            if (permit == null) return NotFound();
            return View(permit);
        }

        // POST: Permits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permit = await _context.Permits.FindAsync(id);
            if (permit != null)
            {
                _context.Permits.Remove(permit);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Permit deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
