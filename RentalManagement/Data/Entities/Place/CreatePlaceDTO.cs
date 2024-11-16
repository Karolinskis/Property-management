namespace RentalManagement.Entities.DTOs;

public class CreatePlaceDTO
{
    /// <example>3</example>
    public int RoomsCount { get; set; }
    /// <example>100</example>
    public int Size { get; set; }
    /// <example>1234 Main St, Springfield, IL 62701</example>
    public string Address { get; set; }
    /// <example>Beautiful place with a view of the park</example>
    public string? Description { get; set; }
    /// <example>1000.00</example>
    public float Price { get; set; }

    public CreatePlaceDTO(int roomsCount, int size, string address, string? description, float price)
    {
        RoomsCount = roomsCount;
        Size = size;
        Address = address;
        Description = description;
        Price = price;
    }
}
