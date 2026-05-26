using FisherMan.ASP.Data;
using Microsoft.AspNetCore.Mvc;

namespace FisherMan.ASP.Controllers
{
    public class TicketsController : Controller
    {
        private readonly FishermanContext _context;

        public TicketsController(FishermanContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.AmateurTickets.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t =>
                    t.FishermanName.Contains(search) ||
                    (t.TelkDecisionNumber != null && t.TelkDecisionNumber.Contains(search)));
            }

            ViewBag.Search = search;
            return View(await query.OrderByDescending(t => t.ValidFrom).ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.AmateurTickets
                .Include(t => t.Catches.OrderByDescending(c => c.CatchDate))
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View(new AmateurTicket { ValidFrom = DateTime.Today, ValidUntil = DateTime.Today.AddYears(1) });
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AmateurTicket ticket)
        {
            if (ModelState.IsValid)
            {
                // Auto-calculate price
                ticket.Price = AmateurTicket.CalculatePrice(ticket.IsUnder14, ticket.IsPensioner, ticket.IsDisabled);
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Ticket issued for {ticket.FishermanName}. Price: {ticket.Price:F2} BGN";
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.AmateurTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AmateurTicket ticket)
        {
            if (id != ticket.Id) return NotFound();

            if (ModelState.IsValid)
            {
                ticket.Price = AmateurTicket.CalculatePrice(ticket.IsUnder14, ticket.IsPensioner, ticket.IsDisabled);
                _context.Update(ticket);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ticket updated.";
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/AddCatch/5
        public async Task<IActionResult> AddCatch(int? id)
        {
            if (id == null) return NotFound();

            var ticket = await _context.AmateurTickets.FindAsync(id);
            if (ticket == null) return NotFound();

            ViewBag.Ticket = ticket;
            return View(new AmateurCatch { AmateurTicketId = id.Value, CatchDate = DateTime.Today });
        }

        // POST: Tickets/AddCatch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCatch(AmateurCatch catch_)
        {
            if (ModelState.IsValid)
            {
                _context.Add(catch_);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Catch recorded.";
                return RedirectToAction(nameof(Details), new { id = catch_.AmateurTicketId });
            }
            ViewBag.Ticket = await _context.AmateurTickets.FindAsync(catch_.AmateurTicketId);
            return View(catch_);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var ticket = await _context.AmateurTickets.FirstOrDefaultAsync(t => t.Id == id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.AmateurTickets.FindAsync(id);
            if (ticket != null)
            {
                _context.AmateurTickets.Remove(ticket);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ticket deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX: Calculate Price
        [HttpGet]
        public IActionResult CalculatePrice(bool isUnder14, bool isPensioner, bool isDisabled)
        {
            var price = AmateurTicket.CalculatePrice(isUnder14, isPensioner, isDisabled);
            return Json(new { price });
        }
    }
}
