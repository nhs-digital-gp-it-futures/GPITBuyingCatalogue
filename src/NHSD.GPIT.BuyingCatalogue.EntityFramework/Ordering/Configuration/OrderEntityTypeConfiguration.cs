using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "ordering");

            builder.Property(o => o.CallOffId)
                .IsRequired()
                .HasMaxLength(4000)
                .HasComputedColumnSql("(concat('C',format([Id],'000000'),'-',format([Revision],'00')))", false)
                .HasConversion(id => id.ToString(), id => CallOffId.Parse(id).Id);

            builder.Property(o => o.Completed);
            builder.Property(o => o.CommencementDate).HasColumnType("date");
            builder.Property(o => o.Created).HasDefaultValueSql("(getutcdate())");

            builder.Property(o => o.Description)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.FundingSourceOnlyGms).HasColumnName("FundingSourceOnlyGMS");
            builder.Property(o => o.LastUpdated).HasDefaultValueSql("(getutcdate())");
            builder.Property(o => o.LastUpdatedBy);
            builder.Property(o => o.LastUpdatedByName).HasMaxLength(256);
            builder.Property(o => o.OrderStatus)
                .HasConversion<int>()
                .HasColumnName("OrderStatusId");
            builder.Property(o => o.Revision).HasDefaultValueSql("((1))");
            builder.Property(o => o.SupplierId).HasMaxLength(6);

            builder.HasOne(o => o.OrderingPartyContact)
                .WithMany(c => c.OrderOrderingPartyContacts)
                .HasForeignKey(o => o.OrderingPartyContactId)
                .HasConstraintName("FK_Order_OrderingPartyContact");

            builder.HasOne(o => o.OrderingParty)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.OrderingPartyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_OrderingParty");

            builder.HasOne(o => o.SupplierContact)
                .WithMany(c => c.OrderSupplierContacts)
                .HasForeignKey(o => o.SupplierContactId)
                .HasConstraintName("FK_Order_SupplierContact");

            builder.HasOne(o => o.Supplier)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.SupplierId)
                .HasConstraintName("FK_Order_Supplier");

            builder.HasIndex(o => o.IsDeleted, "IX_Order_IsDeleted");

            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
