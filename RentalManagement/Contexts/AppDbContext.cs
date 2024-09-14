using Microsoft.EntityFrameworkCore;
using RentalManagement.Entities;

namespace RentalManagement.Contexts;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<Place> Places { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
    }
}
