using System.ComponentModel.DataAnnotations;

namespace RentalManagement.Entities;

public class Review
{
    public int Id { get; set; }
    public int ReservationId { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Comment is required.")]
    public required string Comment { get; set; }
}
