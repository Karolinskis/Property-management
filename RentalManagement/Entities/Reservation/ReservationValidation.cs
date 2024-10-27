using FluentValidation;

using RentalManagement.Entities.DTOs;

namespace RentalManagement.Entities.Validation;

public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDTO>
{
    public CreateReservationDtoValidator()
    {
        RuleFor(o => o.StartDate).NotNull().NotEmpty();
        RuleFor(o => o.EndDate).NotNull().NotEmpty();
    }
}

public class UpdateReservationDtoValidator : AbstractValidator<CreateReservationDTO>
{
    public UpdateReservationDtoValidator()
    {
        RuleFor(o => o.StartDate).NotNull().NotEmpty();
        RuleFor(o => o.EndDate).NotNull().NotEmpty();
    }
}
