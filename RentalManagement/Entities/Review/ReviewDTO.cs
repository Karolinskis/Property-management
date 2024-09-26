namespace RentalManagement.Entities.DTOs;

public record ReviewDto(int Id, int ReservationId, int Rating, string Comment);
public record CreateReviewDto(int ReservationId, int Rating, string Comment);
public record UpdateReviewDto(int Rating, string Comment);
