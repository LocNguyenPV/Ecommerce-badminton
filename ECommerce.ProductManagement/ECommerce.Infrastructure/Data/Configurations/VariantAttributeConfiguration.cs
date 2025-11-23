using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Data.Configurations;

public class VariantAttributeConfiguration : IEntityTypeConfiguration<VariantAttribute>
{
    public void Configure(EntityTypeBuilder<VariantAttribute> builder)
    {
        // PK
        builder.HasKey(va => va.VariantAttributeId);

        // Properties
        builder.Property(va => va.VariantId)
            .IsRequired();

        builder.Property(va => va.AttributeId)
            .IsRequired();

        builder.Property(va => va.AttributeValue)
            .IsRequired()
            .HasMaxLength(500);

        // Index
        builder.HasIndex(va => va.VariantId)
            .HasDatabaseName("IX_VariantAttributes_VariantId");

        builder.HasIndex(va => va.AttributeId)
            .HasDatabaseName("IX_VariantAttributes_AttributeId");

        // FK
        builder.HasOne(va => va.Variant)
            .WithMany(v => v.Attributes)
            .HasForeignKey(va => va.VariantId)
            .HasConstraintName("FK_VariantAttributes_Variants")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(va => va.CustomAttribute)
            .WithMany(a => a.VariantAttributes)
            .HasForeignKey(va => va.AttributeId)
            .HasConstraintName("FK_VariantAttributes_Attributes")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
