using FluentValidation;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Validators;

public class CreateProductImageDtoValidator : AbstractValidator<CreateProductImageDto>
{
    public CreateProductImageDtoValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Image URL is required")
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters")
            .Must(BeValidUrl).WithMessage("Image URL must be a valid URL");

        RuleFor(x => x.AltText)
            .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.AltText))
            .WithMessage("Alt text must not exceed 200 characters");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Sort order must be non-negative");

        RuleFor(x => x)
            .Must(x => x.ProductId.HasValue)
            .WithMessage("ProductId must be specified");
    }

    private bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
