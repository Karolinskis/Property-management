namespace RentalManagement.Entities.DTOs;

public class CreateReviewDTO
{
    /// <example>5</example>
    public int Rating { get; set; }
    /// <example>Great place!</example>
    public string Comment { get; set; }

    public CreateReviewDTO(int rating, string comment)
    {
        Rating = rating;
        Comment = comment;
    }
}
