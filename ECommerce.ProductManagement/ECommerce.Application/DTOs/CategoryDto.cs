namespace ECommerce.Application.DTOs;

/// <summary>
/// Category information
/// </summary>
public record CategoryDto
{
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public Guid? ParentCategoryId { get; init; }
    public string? ParentCategoryName { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public int SortOrder { get; init; }
    public int ProductCount { get; init; }
}

/// <summary>
/// Hierarchical category with subcategories
/// </summary>
public record CategoryHierarchyDto
{
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public Guid? ParentCategoryId { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public int SortOrder { get; init; }
    public ICollection<CategoryHierarchyDto> SubCategories { get; init; } = new List<CategoryHierarchyDto>();
}

/// <summary>
/// Create category request
/// </summary>
public record CreateCategoryDto
{
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public Guid? ParentCategoryId { get; init; }
    public string? Description { get; init; }
    public int SortOrder { get; init; }
}

/// <summary>
/// Update category request
/// </summary>
public record UpdateCategoryDto
{
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public Guid? ParentCategoryId { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public int SortOrder { get; init; }
}
