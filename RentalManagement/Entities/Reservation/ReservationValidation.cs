using FluentValidation;

using RentalManagement.Entities.DTOs;

namespace RentalManagement.Entities.Validation;

public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationDtoValidator()
    {
        RuleFor(o => o.PlaceId).GreaterThan(0).NotNull().NotEmpty();
        RuleFor(o => o.StartDate).NotNull().NotEmpty();
        RuleFor(o => o.EndDate).NotNull().NotEmpty();
    }
}

public class UpdateReservationDtoValidator : AbstractValidator<UpdateReservationDto>
{
    public UpdateReservationDtoValidator()
    {
        RuleFor(o => o.StartDate).NotNull().NotEmpty();
        RuleFor(o => o.EndDate).NotNull().NotEmpty();
    }
}