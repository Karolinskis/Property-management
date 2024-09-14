using System.ComponentModel.DataAnnotations;

namespace RentalManagement.Entities;

public class Review
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
