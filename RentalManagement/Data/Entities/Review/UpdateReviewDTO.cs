namespace RentalManagement.Entities.DTOs;

public class UpdateReviewDTO
{
    /// <example>5</example>
    public int Rating { get; set; }
    /// <example>Great place!</example>
    public string Comment { get; set; }

    public UpdateReviewDTO(int rating, string comment)
    {
        Rating = rating;
        Comment = comment;
    }
}
