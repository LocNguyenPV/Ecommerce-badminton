using FluentValidation;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Validators;

public class UpdateInventoryDtoValidator : AbstractValidator<UpdateInventoryDto>
{
    public UpdateInventoryDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative")
            .LessThanOrEqualTo(100000).WithMessage("Quantity must not exceed 100,000");

        RuleFor(x => x.Operation)
            .IsInEnum().WithMessage("Invalid inventory operation");

        RuleFor(x => x.Reason)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Reason))
            .WithMessage("Reason must not exceed 500 characters");
    }
}
