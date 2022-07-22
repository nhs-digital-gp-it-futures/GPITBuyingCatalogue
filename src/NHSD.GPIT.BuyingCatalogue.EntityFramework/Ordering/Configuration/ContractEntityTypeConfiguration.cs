﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class ContractEntityTypeConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contracts", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_Contracts");

            builder.Property(x => x.OrderId).IsRequired();

            builder.HasOne(x => x.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.LastUpdatedBy)
                .HasConstraintName("FK_Contracts_LastUpdatedBy");

            builder.HasOne(x => x.DataProcessingPlan)
                .WithMany()
                .HasForeignKey(x => x.DataProcessingPlanId)
                .HasConstraintName("FK_Contracts_DataProcessingPlan");

            builder.HasOne(x => x.ImplementationPlan)
                .WithMany()
                .HasForeignKey(x => x.ImplementationPlanId)
                .HasConstraintName("FK_Contracts_ImplementationPlan");

            builder.HasMany(x => x.BillingItems)
                .WithOne(x => x.Contract)
                .HasForeignKey(x => x.ContractId)
                .HasConstraintName("FK_ContractBillingItems_Contract");
        }
    }
}
