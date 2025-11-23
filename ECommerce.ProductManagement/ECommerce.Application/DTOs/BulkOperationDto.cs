namespace ECommerce.Application.DTOs;

/// <summary>
/// Bulk price update request
/// </summary>
public record BulkUpdatePriceDto
{
    public IEnumerable<UpdatePriceDto> Updates { get; init; } = Enumerable.Empty<UpdatePriceDto>();
}

public record UpdatePriceDto
{
    public Guid ProductId { get; init; }
    public decimal NewPrice { get; init; }
    public decimal? NewCostPrice { get; init; }
}

/// <summary>
/// Bulk inventory update request
/// </summary>
public record BulkUpdateInventoryDto
{
    public IEnumerable<UpdateInventoryItemDto> Updates { get; init; } = Enumerable.Empty<UpdateInventoryItemDto>();
}

public record UpdateInventoryItemDto
{
    public Guid VariantId { get; init; }
    public int Quantity { get; init; }
    public InventoryOperation Operation { get; init; }
}

/// <summary>
/// Bulk operation result
/// </summary>
public record BulkOperationResult
{
    public int TotalItems { get; init; }
    public int SuccessCount { get; init; }
    public int FailureCount { get; init; }
    public IEnumerable<BulkOperationError> Errors { get; init; } = Enumerable.Empty<BulkOperationError>();
}

public record BulkOperationError
{
    public Guid ItemId { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
}
