using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderProgressEntityTypeConfiguration : IEntityTypeConfiguration<OrderProgress>
    {
        public void Configure(EntityTypeBuilder<OrderProgress> builder)
        {
            builder.ToTable("OrderProgress", "ordering");

            builder.HasKey(op => op.OrderId);

            builder.Property(op => op.OrderId).ValueGeneratedNever();

            builder.HasOne(op => op.Order)
                .WithOne(o => o.Progress)
                .HasForeignKey<OrderProgress>(op => op.OrderId)
                .HasConstraintName("FK_OrderProgress_Order");
        }
    }
}
