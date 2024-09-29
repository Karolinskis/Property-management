using Swashbuckle.AspNetCore.Annotations;

namespace RentalManagement.Entities.DTOs;

public class CreateReservationDTO
{
    /// <example>2021-09-01T00:00:00</example>
    public DateTime StartDate { get; set; }
    /// <example>2021-09-01T00:00:00</example>
    public DateTime EndDate { get; set; }
    /// <example>100.0</example>
    public float Price { get; set; }
}
