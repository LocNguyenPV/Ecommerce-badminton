using FluentValidation;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Validators;

public class ProductQueryParametersValidator : AbstractValidator<ProductQueryParameters>
{
    private static readonly string[] AllowedSortFields = { "name", "basePrice", "createdAt", "updatedAt", "sku" };
    private static readonly string[] AllowedSortOrders = { "asc", "desc" };

    public ProductQueryParametersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
            .WithMessage("Search term must not exceed 200 characters");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue)
            .WithMessage("Minimum price must be non-negative");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice ?? 0).When(x => x.MaxPrice.HasValue)
            .WithMessage("Maximum price must be greater than or equal to minimum price");

        RuleFor(x => x.SortBy)
            .Must(x => AllowedSortFields.Contains(x.ToLower()))
            .WithMessage($"Sort field must be one of: {string.Join(", ", AllowedSortFields)}");

        RuleFor(x => x.SortOrder)
            .Must(x => AllowedSortOrders.Contains(x.ToLower()))
            .WithMessage("Sort order must be either 'asc' or 'desc'");
    }
}
