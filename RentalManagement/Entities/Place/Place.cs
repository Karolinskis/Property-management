using System.ComponentModel.DataAnnotations;

namespace RentalManagement.Entities;

public class Place
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "There must be at least 1 room.")]
    public required int RoomsCount { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Size must be greater than 0.")]
    public required int Size { get; set; }
    public required string Address { get; set; }
    public string? Description { get; set; }
    [Range(0, float.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public required float Price { get; set; }
}

