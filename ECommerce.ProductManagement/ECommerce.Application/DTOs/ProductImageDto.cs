namespace ECommerce.Application.DTOs;

/// <summary>
/// Product image information
/// </summary>
public record ProductImageDto
{
    public Guid ImageId { get; init; }
    public Guid? ProductId { get; init; }
    public string Url { get; init; } = string.Empty;
    public string? AltText { get; init; }
    public int SortOrder { get; init; }
    public bool IsPrimary { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create product image request
/// </summary>
public record CreateProductImageDto
{
    public Guid? ProductId { get; init; }
    public string Url { get; init; } = string.Empty;
    public string? AltText { get; init; }
    public int SortOrder { get; init; }
    public bool IsPrimary { get; init; }
}

/// <summary>
/// Update product image request
/// </summary>
public record UpdateProductImageDto
{
    public Guid ImageId { get; init; }

    public string? AltText { get; init; }
    public int SortOrder { get; init; }
    public bool IsPrimary { get; init; }
}
