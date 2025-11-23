namespace ECommerce.Application.DTOs;

/// <summary>
/// Validation error details
/// </summary>
public record ValidationErrorDto
{
    public string PropertyName { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
    public object? AttemptedValue { get; init; }
}

/// <summary>
/// Validation result with detailed errors
/// </summary>
public record ValidationResultDto
{
    public bool IsValid { get; init; }
    public IEnumerable<ValidationErrorDto> Errors { get; init; } = Enumerable.Empty<ValidationErrorDto>();
}
