using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductVariantConfig : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        //PK
        builder.HasKey(p => p.VariantId);

        // Properties
        builder.Property(v => v.ProductId)
            .IsRequired();
        builder.Property(p => p.RowVersion)
            .IsRowVersion();

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.VariantName).HasMaxLength(100);
        builder.Property(p => p.PriceAdjustment).HasPrecision(18, 2);
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.ReservedQuantity).IsRequired();

        //FK
        builder.HasOne(v => v.Product)
             .WithMany(p => p.Variants)
             .HasForeignKey(v => v.ProductId)
             .HasConstraintName("FK_ProductVariants_Products")
             .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Attributes)
            .WithOne(a => a.Variant)
            .HasForeignKey(a => a.VariantId)
            .HasConstraintName("FK_VariantAttributes_Variants")
            .OnDelete(DeleteBehavior.Cascade);

        //Index
        builder.HasIndex(v => v.SKU)
            .IsUnique()
            .HasDatabaseName("IX_ProductVariants_SKU");

        builder.HasIndex(v => v.ProductId)
            .HasDatabaseName("IX_ProductVariants_ProductId");

        // Check Constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CHK_StockQuantity", "StockQuantity >= 0");
            t.HasCheckConstraint("CHK_ReservedQuantity", "ReservedQuantity >= 0 AND ReservedQuantity <= StockQuantity");
        });
    }
}
