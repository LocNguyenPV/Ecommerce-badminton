namespace ECommerce.Application.DTOs;

public class ProductQueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsActive { get; set; } = true;
    public string SortBy { get; set; } = "createdAt";
    public string SortOrder { get; set; } = "desc";
    public string? Fields { get; set; }
}
