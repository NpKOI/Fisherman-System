using FisherMan.ASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class PersonsController : Controller
    {
        private readonly FishermanContext _context;

        public PersonsController(FishermanContext context)
        {
            _context = context;
        }

        // GET: Persons
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Persons.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.FullName.Contains(search) ||
                    (p.IdentificationNumber != null && p.IdentificationNumber.Contains(search)) ||
                    (p.CompanyName != null && p.CompanyName.Contains(search)));
            }

            return View(await query.OrderBy(p => p.FullName).ToListAsync());
        }

        // GET: Persons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var person = await _context.Persons
                .Include(p => p.OwnedVessels)
                .Include(p => p.Permits).ThenInclude(pr => pr.Vessel)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null) return NotFound();

            return View(person);
        }

        // GET: Persons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Persons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Person created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: Persons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound();

            return View(person);
        }

        // POST: Persons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Person person)
        {
            if (id != person.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Person updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: Persons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Id == id);
            if (person == null) return NotFound();

            return View(person);
        }

        // POST: Persons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person != null)
            {
                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Person deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id) => _context.Persons.Any(e => e.Id == id);
    }
}
