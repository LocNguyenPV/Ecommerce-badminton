using FluentValidation;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Validators;

public class CreateVariantAttributeDtoValidator : AbstractValidator<CreateVariantAttributeDto>
{
    public CreateVariantAttributeDtoValidator()
    {
        RuleFor(x => x.AttributeId)
            .NotEmpty().WithMessage("Attribute ID is required");

        RuleFor(x => x.AttributeValue)
            .NotEmpty().WithMessage("Attribute value is required")
            .MaximumLength(500).WithMessage("Attribute value must not exceed 500 characters");
    }
}
