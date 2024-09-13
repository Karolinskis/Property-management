using System.ComponentModel.DataAnnotations;

namespace RentalManagement.Models;

public class Rent
{
    public DateTime Date { get; set; }
    public Status Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
