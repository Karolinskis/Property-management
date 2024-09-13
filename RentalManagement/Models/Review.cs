using System.ComponentModel.DataAnnotations;

namespace RentalManagement.Models;

public class Review
{
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
