using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", Schemas.Ordering);

            builder.Property(o => o.Completed);
            builder.Property(o => o.CommencementDate).HasColumnType("date");
            builder.Property(o => o.Created).HasDefaultValue(DateTime.UtcNow);

            builder.Property(o => o.Description)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.FundingSourceOnlyGms).HasColumnName("FundingSourceOnlyGMS");
            builder.Property(o => o.LastUpdated).HasDefaultValue(DateTime.UtcNow);
            builder.Property(o => o.LastUpdatedBy);
            builder.Property(o => o.OrderStatus)
                .HasConversion<int>()
                .HasColumnName("OrderStatusId");

            builder.Property(o => o.SupplierId).HasMaxLength(6);

            builder.Ignore(o => o.LastUpdatedByName);

            builder.HasOne(o => o.OrderingPartyContact)
                .WithMany(c => c.OrderOrderingPartyContacts)
                .HasForeignKey(o => o.OrderingPartyContactId)
                .HasConstraintName("FK_Orders_OrderingPartyContact");

            builder.HasOne(o => o.OrderingParty)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.OrderingPartyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_OrderingParty");

            builder.HasOne(o => o.SupplierContact)
                .WithMany(c => c.OrderSupplierContacts)
                .HasForeignKey(o => o.SupplierContactId)
                .HasConstraintName("FK_Orders_SupplierContact");

            builder.HasOne(o => o.Supplier)
                .WithMany()
                .HasForeignKey(o => o.SupplierId)
                .HasConstraintName("FK_Orders_Supplier");

            builder.HasOne(o => o.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(o => o.LastUpdatedBy)
                .HasConstraintName("FK_Orders_LastUpdatedBy");

            builder.HasIndex(o => o.IsDeleted, "IX_Orders_IsDeleted");

            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
