using FluentValidation;

namespace RentalManagement.Entities.DTOs;

public record PlaceDto(int Id, int RoomsCount, int Size, string Address, string? Description, float Price);
public record CreatePlaceDto(int RoomsCount, int Size, string Address, string? Description, float Price);
public record UpdatePlaceDto(int RoomsCount, int Size, string Address, string? Description, float Price);
