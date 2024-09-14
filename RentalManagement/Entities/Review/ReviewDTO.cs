namespace RentalManagement.Entities.DTOs;

public record ReviewDto(int Id, int ReservationId, int Rating, string Comment);
public record CreateReviewDto(int ReservationId, int Rating, string Comment);
public record UpdateReviewDto(int ReservationId, int Rating, string Comment);
