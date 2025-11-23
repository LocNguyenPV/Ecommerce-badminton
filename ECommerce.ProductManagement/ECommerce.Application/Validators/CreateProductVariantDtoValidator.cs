using FluentValidation;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Validators;

public class CreateProductVariantDtoValidator : AbstractValidator<CreateProductVariantDto>
{
    public CreateProductVariantDtoValidator()
    {
        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("Variant SKU is required")
            .MaximumLength(50).WithMessage("Variant SKU must not exceed 50 characters")
            .Matches("^[A-Z0-9-]+$").WithMessage("Variant SKU must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.VariantName)
            .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.VariantName))
            .WithMessage("Variant name must not exceed 100 characters");

        RuleFor(x => x.PriceAdjustment)
            .ScalePrecision(2, 18).WithMessage("Price adjustment must have at most 2 decimal places")
            .GreaterThanOrEqualTo(-10000).WithMessage("Price adjustment must not be less than -10,000");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be non-negative");

        RuleForEach(x => x.Attributes)
            .SetValidator(new CreateVariantAttributeDtoValidator());
    }
}
