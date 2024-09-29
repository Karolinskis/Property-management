using FluentValidation;

using RentalManagement.Entities.DTOs;

namespace RentalManagement.Entities.Validation;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDTO>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(o => o.Rating).InclusiveBetween(1, 5).NotNull().NotEmpty();
    }
}

public class UpdateReviewDtoValidator : AbstractValidator<UpdateReviewDTO>
{
    public UpdateReviewDtoValidator()
    {
        RuleFor(o => o.Rating).InclusiveBetween(1, 5).NotNull().NotEmpty();
    }
}
