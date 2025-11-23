using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Data.Configurations;

public class CustomAttributeConfiguration : IEntityTypeConfiguration<ECommerce.Domain.Entities.CustomAttribute>
{
    public void Configure(EntityTypeBuilder<ECommerce.Domain.Entities.CustomAttribute> builder)
    {
        // Primary Key
        builder.HasKey(a => a.AttributeId);

        // Properties
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);


        // Relationships
        builder.HasMany(a => a.VariantAttributes)
            .WithOne(va => va.CustomAttribute)
            .HasForeignKey(va => va.AttributeId)
            .HasConstraintName("FK_VariantAttributes_Attributes")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
