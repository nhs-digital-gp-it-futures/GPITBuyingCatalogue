using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderDeletionApprovalEntityTypeConfiguration : IEntityTypeConfiguration<OrderDeletionApproval>
    {
        public void Configure(EntityTypeBuilder<OrderDeletionApproval> builder)
        {
            builder.ToTable("OrderDeletionApprovals", Schemas.Ordering);

            builder.HasKey(a => a.Id).HasName("PK_OrderDeletionApproval");

            builder.Property(x => x.OrderId).IsRequired();
            builder.Property(a => a.NameOfApprover).IsRequired();
            builder.Property(a => a.NameOfRequester).IsRequired();
            builder.Property(a => a.DateOfApproval).IsRequired();

            builder.Property(a => a.LastUpdated)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.Property(a => a.LastUpdatedBy)
                .IsRequired();

            builder.HasOne(a => a.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(a => a.LastUpdatedBy)
                .HasConstraintName("FK_OrderDeletionApprovals_LastUpdatedBy");

            builder.HasOne(a => a.Order)
                .WithOne(o => o.OrderDeletionApproval)
                .HasForeignKey<OrderDeletionApproval>(d => d.OrderId)
                .HasConstraintName("FK_OrderDeletionApprovals_Order");
        }
    }
}
