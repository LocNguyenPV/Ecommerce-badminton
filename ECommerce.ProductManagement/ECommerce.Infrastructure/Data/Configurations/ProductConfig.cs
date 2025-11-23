using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        //PK
        builder.HasKey(p => p.ProductId);

        // Properties
        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.BasePrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.CostPrice)
            .HasPrecision(18, 2);

        builder.Property(p => p.RowVersion)
            .IsRowVersion();

        //FK
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .HasConstraintName("FK_Products_Categories")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .HasConstraintName("FK_Products_Brands")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId)
            .HasConstraintName("FK_ProductVariants_Products")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .HasConstraintName("FK_ProductImages_Products")
            .OnDelete(DeleteBehavior.Cascade);


        //Index
        builder.HasIndex(p => p.SKU).IsUnique().HasDatabaseName("IX_Products_SKU");
        builder.HasIndex(p => p.CategoryId).HasDatabaseName("IX_Products_CategoryId");

        builder.HasIndex(p => p.BrandId).HasDatabaseName("IX_Products_BrandId");
        builder.HasIndex(p => p.IsActive).HasDatabaseName("IX_Products_IsActive");

    }
}
