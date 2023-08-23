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

            builder.HasKey(x => x.Id).HasName("PK_Orders");

            builder.Property(x => x.OrderNumber); // .IsRequired();
            builder.Property(x => x.Revision).HasDefaultValue(1); // .IsRequired();
            builder.Property(o => o.Completed);
            builder.Property(o => o.CommencementDate).HasColumnType("date");
            builder.Property(o => o.Created).HasDefaultValue(DateTime.UtcNow);
            builder.Property(o => o.IsTerminated);

            builder.Property(o => o.Description)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.OrderTriageValue)
                .HasConversion<int>()
                .HasColumnName("OrderTriageValueId");

            builder.Property(o => o.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.Property(o => o.SupplierId).HasMaxLength(6);

            builder.HasOne(o => o.OrderingPartyContact)
                .WithMany()
                .HasForeignKey(o => o.OrderingPartyContactId)
                .HasConstraintName("FK_Orders_OrderingPartyContact");

            builder.HasOne(o => o.OrderingParty)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.OrderingPartyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_OrderingParty");

            builder.HasOne(o => o.SupplierContact)
                .WithMany()
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

            builder.HasOne(x => x.Solution)
                .WithMany()
                .HasForeignKey(x => x.SolutionId)
                .HasConstraintName("FK_Orders_Solution");

            builder.HasOne(o => o.SelectedFramework)
                .WithMany()
                .HasForeignKey(o => o.SelectedFrameworkId)
                .HasConstraintName("FK_Orders_SelectedFramework");

            builder.HasIndex(o => o.IsDeleted, "IX_Orders_IsDeleted");

            builder.HasIndex(
                o => new
                {
                    o.OrderNumber,
                    o.IsDeleted,
                    o.Revision,
                    o.OrderingPartyId,
                    o.SolutionId,
                },
                "IX_OrderNum_IsDeleted_Revision");

            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
