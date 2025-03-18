using CineTicket.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Users> Users { get; set; }
    public DbSet<Tickets> Tickets { get; set; }
    public DbSet<Showtimes> Showtimes { get; set; }
    public DbSet<Poster> Posters { get; set; }
}
