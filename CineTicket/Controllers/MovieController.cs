using Microsoft.AspNetCore.Mvc;

public class MovieController : Controller
{
    private readonly ApplicationDbContext _context;
    public MovieController(ApplicationDbContext context) { _context = context; }

    public IActionResult Index()
    {
        var movies = _context.Movies.ToList();
        return View(movies);
    }

    public IActionResult Details(int id)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }


}
