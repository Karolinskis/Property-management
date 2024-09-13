using Microsoft.EntityFrameworkCore;
using RentalManagement.Models;

namespace RentalManagement.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Place> Places { get; set; } = null!;
}
