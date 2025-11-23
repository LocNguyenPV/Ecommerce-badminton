namespace ECommerce.Application.DTOs;

/// <summary>
/// Brand information
/// </summary>
public record BrandDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? LogoUrl { get; init; }
    public bool IsActive { get; init; }
    public int ProductCount { get; init; }
}

/// <summary>
/// Create brand request
/// </summary>
public record CreateBrandDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? LogoUrl { get; init; }
}

/// <summary>
/// Update brand request
/// </summary>
public record UpdateBrandDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? LogoUrl { get; init; }
    public bool IsActive { get; init; }
}
