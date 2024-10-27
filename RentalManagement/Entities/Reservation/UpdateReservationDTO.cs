using Swashbuckle.AspNetCore.Annotations;

namespace RentalManagement.Entities.DTOs;

public class UpdateReservationDTO
{
    /// <example>2021-09-01T00:00:00Z</example>
    public DateTime StartDate { get; set; }
    /// <example>2021-09-01T00:00:00Z</example>
    public DateTime EndDate { get; set; }
    /// <example>Pending</example>
    public string Status { get; set; }
    /// <example>100.0</example>
    public float Price { get; set; }

    public UpdateReservationDTO(DateTime startDate, DateTime endDate, string status, float price)
    {
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        Price = price;
    }
}
