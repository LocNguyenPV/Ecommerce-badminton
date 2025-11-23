using FluentValidation;
using ECommerce.Application.DTOs;
using ECommerce.Application.Interfaces;

namespace ECommerce.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    private readonly IProductRepository _productRepository;

    public CreateProductDtoValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters")
            .Matches("^[A-Z0-9-]+$").WithMessage("SKU must contain only uppercase letters, numbers, and hyphens")
            .MustAsync(BeUniqueSKU).WithMessage("SKU already exists");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters")
            .MinimumLength(3).WithMessage("Product name must be at least 3 characters");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage("Base price must be greater than 0")
            .LessThan(1000000).WithMessage("Base price must be less than 1,000,000")
            .ScalePrecision(2, 18).WithMessage("Price must have at most 2 decimal places");

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0).When(x => x.CostPrice.HasValue)
            .WithMessage("Cost price must be non-negative")
            .LessThanOrEqualTo(x => x.BasePrice).When(x => x.CostPrice.HasValue)
            .WithMessage("Cost price should not exceed base price");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");

        //RuleFor(x => x.Variants)
        //    .Must(variants => variants == null || variants.Count > 0)
        //    .When(x => x.Variants != null)
        //    .WithMessage("If variants are provided, at least one variant is required");

        //RuleForEach(x => x.Variants)
        //    .SetValidator(new CreateProductVariantDtoValidator());

        //RuleForEach(x => x.Images)
        //    .SetValidator(new CreateProductImageDtoValidator());
    }

    private async Task<bool> BeUniqueSKU(string sku, CancellationToken cancellationToken)
    {
        return !await _productRepository.ExistsBySKUAsync(sku, cancellationToken);
    }
}
