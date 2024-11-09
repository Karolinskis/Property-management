namespace RentalManagement.Entities.DTOs;

public class ReviewDTO
{
    /// <example>1</example>
    public int Id { get; set; }
    /// <example>1</example>
    public int ReservationId { get; set; }
    /// <example>5</example>
    public int Rating { get; set; }
    /// <example>Great place!</example>
    public string Comment { get; set; }

    public ReviewDTO(int id, int reservationId, int rating, string comment)
    {
        Id = id;
        ReservationId = reservationId;
        Rating = rating;
        Comment = comment;
    }
}
