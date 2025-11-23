using AutoMapper;
using ECommerce.Domain.Entities;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Mappings;

/// <summary>
/// Master mapping profile for all entity-DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ConfigureProductMappings();
        ConfigureProductVariantMappings();
        ConfigureVariantAttributeMappings();
        ConfigureAttributeMappings();
        ConfigureCategoryMappings();
        ConfigureBrandMappings();
        ConfigureProductImageMappings();
    }

    private void ConfigureProductMappings()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
            .ForMember(d => d.BrandName, opt => opt.MapFrom(s => s.Brand != null ? s.Brand.Name : null))
            .ForMember(d => d.PrimaryImageUrl, opt => opt.MapFrom(s =>
                s.Images.Where(i => i.IsPrimary).OrderBy(i => i.SortOrder).Select(i => i.Url).FirstOrDefault()));

        CreateMap<Product, ProductDetailDto>()
            .IncludeBase<Product, ProductDto>()
            .ForMember(d => d.Variants, opt => opt.MapFrom(s => s.Variants.Where(v => v.IsActive)))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images.OrderBy(i => i.SortOrder)));

        CreateMap<CreateProductDto, Product>()
            .ForMember(d => d.ProductId, opt => opt.Ignore())
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => true))
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.Category, opt => opt.Ignore())
            .ForMember(d => d.Brand, opt => opt.Ignore())
            .ForMember(d => d.Variants, opt => opt.Ignore())
            .ForMember(d => d.Images, opt => opt.Ignore())
            .ForMember(d => d.RowVersion, opt => opt.Ignore());

        CreateMap<UpdateProductDto, Product>()
            .ForMember(d => d.ProductId, opt => opt.Ignore())
            .ForMember(d => d.SKU, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => DateTime.Now.Ticks))
            .ForMember(d => d.CreatedBy, opt => opt.Ignore())
            .ForMember(d => d.UpdatedBy, opt => opt.Ignore())
            .ForMember(d => d.Category, opt => opt.Ignore())
            .ForMember(d => d.Brand, opt => opt.Ignore())
            .ForMember(d => d.Variants, opt => opt.Ignore())
            .ForMember(d => d.Images, opt => opt.Ignore());
    }

    private void ConfigureProductVariantMappings()
    {
        CreateMap<ProductVariant, ProductVariantDto>()
            .ForMember(d => d.FinalPrice, opt => opt.MapFrom(s => s.Product.BasePrice + s.PriceAdjustment))
            .ForMember(d => d.AvailableQuantity, opt => opt.MapFrom(s => s.StockQuantity - s.ReservedQuantity))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => s.Images.OrderBy(i => i.SortOrder)));

        CreateMap<CreateProductVariantDto, ProductVariant>()
            .ForMember(d => d.VariantId, opt => opt.Ignore())
            .ForMember(d => d.ProductId, opt => opt.Ignore())
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => true))
            .ForMember(d => d.ReservedQuantity, opt => opt.MapFrom(s => 0))
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => DateTime.Now.Ticks))
            .ForMember(d => d.RowVersion, opt => opt.Ignore())
            .ForMember(d => d.Images, opt => opt.Ignore())
            .ForMember(d => d.Product, opt => opt.Ignore());
    }

    private void ConfigureVariantAttributeMappings()
    {
        CreateMap<VariantAttribute, VariantAttributeDto>()
            .ForMember(d => d.AttributeName, opt => opt.MapFrom(s => s.CustomAttribute.Name))
            .ForMember(d => d.AttributeType, opt => opt.MapFrom(s => s.CustomAttribute.Type));

        CreateMap<CreateVariantAttributeDto, VariantAttribute>()
            .ForMember(d => d.Variant, opt => opt.Ignore())
            .ForMember(d => d.CustomAttribute, opt => opt.Ignore())
            .ForMember(d => d.VariantAttributeId, opt => opt.Ignore())
            .ForMember(d => d.VariantId, opt => opt.Ignore());
    }

    private void ConfigureAttributeMappings()
    {
        CreateMap<ECommerce.Domain.Entities.CustomAttribute, AttributeDto>();
        CreateMap<CreateAttributeDto, ECommerce.Domain.Entities.CustomAttribute>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.AttributeName))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.AttributeType))
            .ForMember(d => d.VariantAttributes, opt => opt.Ignore())
            .ForMember(d => d.AttributeId, opt => opt.Ignore());

        CreateMap<UpdateAttributeDto, ECommerce.Domain.Entities.CustomAttribute>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.AttributeName))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.AttributeType))
            .ForMember(d => d.VariantAttributes, opt => opt.Ignore())
            .ForMember(d => d.AttributeId, opt => opt.Ignore());
    }

    private void ConfigureCategoryMappings()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ParentCategoryName, opt => opt.MapFrom(s => s.ParentCategory != null ? s.ParentCategory.Name : null))
            .ForMember(d => d.ProductCount, opt => opt.MapFrom(s => s.Products.Count(p => p.IsActive)));

        CreateMap<Category, CategoryHierarchyDto>()
            .ForMember(d => d.SubCategories, opt => opt.MapFrom(s => s.SubCategories.Where(c => c.IsActive).OrderBy(c => c.SortOrder)));

        CreateMap<CreateCategoryDto, Category>()
            .ForMember(d => d.CategoryId, opt => opt.Ignore())
            .ForMember(d => d.ParentCategory, opt => opt.Ignore())
            .ForMember(d => d.SubCategories, opt => opt.Ignore())
            .ForMember(d => d.Products, opt => opt.Ignore())
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => true));

        CreateMap<UpdateCategoryDto, Category>()
                        .ForMember(d => d.ParentCategory, opt => opt.Ignore())
            .ForMember(d => d.SubCategories, opt => opt.Ignore())
            .ForMember(d => d.Products, opt => opt.Ignore())
            .ForMember(d => d.CategoryId, opt => opt.Ignore());
    }

    private void ConfigureBrandMappings()
    {
        CreateMap<Brand, BrandDto>()
            .ForMember(d => d.ProductCount, opt => opt.MapFrom(s => s.Products.Count(p => p.IsActive)));

        CreateMap<CreateBrandDto, Brand>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Products, opt => opt.Ignore())
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => true));

        CreateMap<UpdateBrandDto, Brand>()
             .ForMember(d => d.Products, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore());
    }

    private void ConfigureProductImageMappings()
    {
        CreateMap<ProductImage, ProductImageDto>();

        CreateMap<CreateProductImageDto, ProductImage>()
            .ForMember(d => d.ImageId, opt => opt.Ignore())
            .ForMember(d => d.Product, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        CreateMap<UpdateProductImageDto, ProductImage>()
            .ForMember(d => d.ImageId, opt => opt.Ignore())
            .ForMember(d => d.Url, opt => opt.Ignore())
            .ForMember(d => d.ProductId, opt => opt.Ignore())
            .ForMember(d => d.Product, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());
    }
}
