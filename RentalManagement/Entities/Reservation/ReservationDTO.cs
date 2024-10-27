using Swashbuckle.AspNetCore.Annotations;

namespace RentalManagement.Entities.DTOs;

public class ReservationDTO
{

    /// <example>1</example>
    public int Id { get; set; }
    /// <example>1</example>
    public int PlaceId { get; set; }
    /// <example>2021-09-01T00:00:00</example>
    public DateTime CreatedAt { get; set; }
    /// <example>2021-09-01T00:00:00</example>
    public DateTime StartDate { get; set; }
    /// <example>2021-09-01T00:00:00</example>
    public DateTime EndDate { get; set; }
    /// <example>Pending</example>
    public string Status { get; set; }
    /// <example>100.0</example>
    public float Price { get; set; }

    public ReservationDTO(int id, int placeId, DateTime createdAt, DateTime startDate, DateTime endDate, string status, float price)
    {
        Id = id;
        PlaceId = placeId;
        CreatedAt = createdAt;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        Price = price;
    }
}
