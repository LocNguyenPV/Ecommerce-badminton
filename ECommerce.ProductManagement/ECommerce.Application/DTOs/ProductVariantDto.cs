namespace ECommerce.Application.DTOs;

/// <summary>
/// Product variant information
/// </summary>
public record ProductVariantDto
{
    public Guid VariantId { get; init; }
    public Guid ProductId { get; init; }
    public string SKU { get; init; } = string.Empty;
    public string? VariantName { get; init; }
    public decimal PriceAdjustment { get; init; }
    public decimal FinalPrice { get; init; } // Calculated: BasePrice + PriceAdjustment
    public int StockQuantity { get; init; }
    public int ReservedQuantity { get; init; }
    public int AvailableQuantity { get; init; } // Calculated: StockQuantity - ReservedQuantity
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public ICollection<VariantAttributeDto> Attributes { get; init; } = new List<VariantAttributeDto>();
    public ICollection<ProductImageDto> Images { get; init; } = new List<ProductImageDto>();
}

/// <summary>
/// Create product variant request
/// </summary>
public record CreateProductVariantDto
{
    public string SKU { get; init; } = string.Empty;
    public string? VariantName { get; init; }
    public decimal PriceAdjustment { get; init; }
    public int StockQuantity { get; init; }
    public ICollection<CreateVariantAttributeDto>? Attributes { get; init; }
}

/// <summary>
/// Update product variant request
/// </summary>
public record UpdateProductVariantDto
{
    public string? VariantName { get; init; }
    public decimal PriceAdjustment { get; init; }
    public int StockQuantity { get; init; }
    public int ReservedQuantity { get; init; }
    public bool IsActive { get; init; }
    public byte[] RowVersion { get; init; } = Array.Empty<byte>();
}

/// <summary>
/// Inventory update operations
/// </summary>
public record UpdateInventoryDto
{
    public int Quantity { get; init; }
    public InventoryOperation Operation { get; init; }
    public string? Reason { get; init; }
}

public enum InventoryOperation
{
    Add,
    Remove,
    Set,
    Reserve,
    Release
}
