using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class TicketsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TicketsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Showtimes)
                .ThenInclude(s => s.Movie)
            .Include(t => t.User)
            .ToListAsync();

        return View(tickets);
    }

    public async Task<IActionResult> Details(int id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Showtimes)
                .ThenInclude(s => s.Movie)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
            return NotFound();

        return View(ticket);
    }
}
