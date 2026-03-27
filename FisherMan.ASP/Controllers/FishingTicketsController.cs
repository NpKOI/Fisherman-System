using FisherMan.ASP.Data;
using FisherMan.ASP.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FisherMan.ASP.Controllers
{
    public class FishingTicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FishingTicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FishingTickets
        public async Task<IActionResult> Index(string? search)
        {
            ViewData["Search"] = search;
            var tickets = _context.FishingTickets.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                tickets = tickets.Where(t =>
                    t.HolderName.Contains(search) ||
                    t.HolderEGN.Contains(search));
            }
            return View(await tickets.OrderByDescending(t => t.IssueDate).ToListAsync());
        }

        // GET: FishingTickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.FishingTickets
                .Include(t => t.Catches)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // GET: FishingTickets/Create
        public IActionResult Create()
        {
            return View(new FishingTicket { IssueDate = DateTime.Today, ExpiryDate = DateTime.Today.AddYears(1) });
        }

        // POST: FishingTickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HolderName,HolderEGN,BirthDate,IsDisabled,DisabilityDecisionNumber,IsPensioner,ValidityType,IssueDate,ExpiryDate,Price")] FishingTicket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: FishingTickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.FishingTickets.FindAsync(id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        // POST: FishingTickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HolderName,HolderEGN,BirthDate,IsDisabled,DisabilityDecisionNumber,IsPensioner,ValidityType,IssueDate,ExpiryDate,Price")] FishingTicket ticket)
        {
            if (id != ticket.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.FishingTickets.Any(e => e.Id == ticket.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: FishingTickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.FishingTickets.FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // POST: FishingTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.FishingTickets.FindAsync(id);
            if (ticket != null) _context.FishingTickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
