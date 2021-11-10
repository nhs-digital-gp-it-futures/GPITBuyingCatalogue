using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class DefaultDeliveryDateEntityTypeConfiguration : IEntityTypeConfiguration<DefaultDeliveryDate>
    {
        public void Configure(EntityTypeBuilder<DefaultDeliveryDate> builder)
        {
            builder.ToTable("DefaultDeliveryDates", Schemas.Ordering);

            builder.HasKey(d => new { d.OrderId, d.CatalogueItemId })
                .HasName("PK_DefaultDeliveryDates");

            builder.Property(d => d.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(d => d.DeliveryDate).HasColumnType("date");
            builder.Property(d => d.LastUpdated).HasDefaultValue(DateTime.UtcNow);
            builder.Property(d => d.LastUpdatedBy);

            builder.HasOne(d => d.Order)
                .WithMany(o => o.DefaultDeliveryDates)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_DefaultDeliveryDates_Order");

            builder.HasOne(d => d.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(d => d.LastUpdatedBy)
                .HasConstraintName("FK_DefaultDeliveryDates_LastUpdatedBy");
        }
    }
}
