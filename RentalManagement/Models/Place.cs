using System.ComponentModel.DataAnnotations;

namespace RentalManagement.Models;

public class Place
{
    public int Id { get; set; }
    public int RoomsCount { get; set; }
    public int Size { get; set; }
    public int Floor { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public float Price { get; set; }
}

