namespace ECommerce.Domain.Entities;

public class ProductVariant
{
    public Guid VariantId { get; set; }
    public Guid ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? VariantName { get; set; }
    public decimal PriceAdjustment { get; set; }
    public int StockQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public long CreatedAt { get; set; } = DateTime.Now.Ticks;
    public long UpdatedAt { get; set; } = DateTime.Now.Ticks;

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    public Product Product { get; set; } = null!;
    public ICollection<VariantAttribute> Attributes { get; set; } = new List<VariantAttribute>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}
