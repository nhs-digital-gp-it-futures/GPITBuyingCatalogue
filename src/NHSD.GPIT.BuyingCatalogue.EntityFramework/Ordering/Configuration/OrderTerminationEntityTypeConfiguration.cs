﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderTerminationEntityTypeConfiguration : IEntityTypeConfiguration<OrderTermination>
    {
        public void Configure(EntityTypeBuilder<OrderTermination> builder)
        {
            builder.ToTable("OrderTermination", Schemas.Ordering);

            builder.HasKey(a => a.Id).HasName("PK_OrderTermination");

            builder.Property(x => x.OrderId).IsRequired();
            builder.Property(a => a.Reason).IsRequired();
            builder.Property(a => a.DateOfTermination).IsRequired();

            builder.HasOne(a => a.Order)
                .WithOne(o => o.OrderTermination)
                .HasForeignKey<OrderTermination>(d => d.OrderId)
                .HasConstraintName("FK_OrderTermination_Order")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
