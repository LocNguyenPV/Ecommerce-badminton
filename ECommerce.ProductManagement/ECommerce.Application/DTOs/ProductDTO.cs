using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs
{
    public record ProductDto
    {
        public Guid ProductId { get; init; }
        public string SKU { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public decimal BasePrice { get; init; }
        public string CategoryName { get; init; } = string.Empty;
        public string PrimaryImageUrl { get; init; } = string.Empty;
        public string? BrandName { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public record ProductDetailDto : ProductDto
    {
        public decimal? CostPrice { get; init; }
        public CategoryDto Category { get; init; } = null!;
        public BrandDto? Brand { get; init; }
        public ICollection<ProductVariantDto> Variants { get; init; } = new List<ProductVariantDto>();
        public ICollection<ProductImageDto> Images { get; init; } = new List<ProductImageDto>();
        public byte[] RowVersion { get; init; } = Array.Empty<byte>();
    }

    public record CreateProductDto
    {
        public string SKU { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public decimal BasePrice { get; init; }
        public decimal? CostPrice { get; init; }
        public Guid CategoryId { get; init; }
        public Guid? BrandId { get; init; }
    }

    public record UpdateProductDto
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public decimal BasePrice { get; init; }
        public decimal? CostPrice { get; init; }
        public Guid CategoryId { get; init; }
        public Guid? BrandId { get; init; }
        public bool IsActive { get; init; }
        public byte[] RowVersion { get; init; } = Array.Empty<byte>();
    }
}
