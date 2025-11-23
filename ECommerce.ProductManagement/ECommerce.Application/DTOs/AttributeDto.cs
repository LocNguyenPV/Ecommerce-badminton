namespace ECommerce.Application.DTOs;

/// <summary>
/// Attribute definition information
/// </summary>
public record AttributeDto
{
    public Guid AttributeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public int SortOrder { get; init; }
}

/// <summary>
/// Create attribute request
/// </summary>
public record CreateAttributeDto
{
    public string AttributeName { get; init; } = string.Empty;
    public string AttributeType { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public int SortOrder { get; init; }
}

/// <summary>
/// Update attribute request
/// </summary>
public record UpdateAttributeDto
{
    public string AttributeName { get; init; } = string.Empty;
    public string AttributeType { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public int SortOrder { get; init; }
}
