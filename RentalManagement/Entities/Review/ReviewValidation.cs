using FluentValidation;

using RentalManagement.Entities.DTOs;

namespace RentalManagement.Entities.Validation;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(o => o.ReservationId).GreaterThan(0).NotNull().NotEmpty();
        RuleFor(o => o.Rating).InclusiveBetween(1, 5).NotNull().NotEmpty();
    }
}

public class UpdateReviewDtoValidator : AbstractValidator<UpdateReviewDto>
{
    public UpdateReviewDtoValidator()
    {
        RuleFor(o => o.Rating).InclusiveBetween(1, 5).NotNull().NotEmpty();
    }
}
