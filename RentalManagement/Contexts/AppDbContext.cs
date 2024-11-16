using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentalManagement.Auth;
using RentalManagement.Data.Entities;
using RentalManagement.Entities;

namespace RentalManagement.Contexts;

public class AppDbContext : IdentityDbContext<User>
{
    private readonly IConfiguration _configuration;

    public DbSet<Place> Places { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Session> Sessions { get; set; }

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
    }
}
