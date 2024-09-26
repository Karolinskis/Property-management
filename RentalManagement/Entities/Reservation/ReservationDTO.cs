namespace RentalManagement.Entities.DTOs;

public record ReservationDto(int Id, int PlaceId, DateTime CreatedAt, DateTime StartDate, DateTime EndDate, string Status, float Price);
public record CreateReservationDto(int PlaceId, DateTime StartDate, DateTime EndDate, float Price);
public record UpdateReservationDto(DateTime CreatedAt, DateTime StartDate, DateTime EndDate, string Status, float Price);
