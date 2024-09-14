using FluentValidation;

using RentalManagement.Entities.DTOs;

namespace RentalManagement.Entities.Validation;

public class CreatePlaceDtoValidator : AbstractValidator<CreatePlaceDto>
{
    public CreatePlaceDtoValidator()
    {
        RuleFor(o => o.RoomsCount).GreaterThan(0).LessThan(20).NotNull().NotEmpty();
        RuleFor(o => o.Size).GreaterThan(0).LessThan(1000).NotNull().NotEmpty();
        RuleFor(o => o.Address).NotEmpty().NotNull().MinimumLength(5).MaximumLength(100);
        RuleFor(o => o.Price).GreaterThan(0).LessThan(10000).NotEmpty().NotNull();
    }
}

public class UpdatePlaceDtoValidator : AbstractValidator<UpdatePlaceDto>
{
    public UpdatePlaceDtoValidator()
    {
        RuleFor(o => o.RoomsCount).GreaterThan(0).LessThan(20).NotNull().NotEmpty();
        RuleFor(o => o.Size).GreaterThan(0).LessThan(1000).NotNull().NotEmpty();
        RuleFor(o => o.Address).NotEmpty().NotNull().MinimumLength(5).MaximumLength(100);
        RuleFor(o => o.Price).GreaterThan(0).LessThan(10000).NotEmpty().NotNull();
    }
}
