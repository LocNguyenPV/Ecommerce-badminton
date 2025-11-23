namespace ECommerce.Application.DTOs;

/// <summary>
/// Variant attribute information
/// </summary>
public record VariantAttributeDto
{
    public Guid VariantAttributeId { get; init; }
    public Guid VariantId { get; init; }
    public Guid AttributeId { get; init; }
    public string AttributeName { get; init; } = string.Empty;
    public string AttributeType { get; init; } = string.Empty;
    public string AttributeValue { get; init; } = string.Empty;
}

/// <summary>
/// Create variant attribute request
/// </summary>
public record CreateVariantAttributeDto
{
    public Guid AttributeId { get; init; }
    public string AttributeValue { get; init; } = string.Empty;
}

/// <summary>
/// Update variant attribute request
/// </summary>
public record UpdateVariantAttributeDto
{
    public string AttributeValue { get; init; } = string.Empty;
}
