namespace RentalManagement.Entities.DTOs;

public record ReservationDto(int Id, int PlaceId, DateTime CreatedAt, DateTime StartDate, DateTime EndDate, Status Status, float Price);
public record CreateReservationDto(int PlaceId, DateTime CreatedAt, DateTime StartDate, DateTime EndDate, Status Status, float Price);
public record UpdateReservationDto(DateTime CreatedAt, DateTime StartDate, DateTime EndDate, Status Status, float Price);
