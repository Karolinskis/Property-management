namespace RentalManagement.Entities;

public class Place
{
    public int Id { get; set; }

    public required int RoomsCount { get; set; }
    public required int Size { get; set; }
    public required string Address { get; set; }
    public string? Description { get; set; }
    public required float Price { get; set; }
}

