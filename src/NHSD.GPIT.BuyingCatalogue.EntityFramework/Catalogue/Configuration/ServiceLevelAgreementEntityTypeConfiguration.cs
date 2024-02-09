using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class ServiceLevelAgreementEntityTypeConfiguration : IEntityTypeConfiguration<ServiceLevelAgreements>
    {
        public void Configure(EntityTypeBuilder<ServiceLevelAgreements> builder)
        {
            builder.ToTable("ServiceLevelAgreements", Schemas.Catalogue, b => b.IsTemporal(
                temp =>
                {
                    temp.UseHistoryTable("ServiceLevelAgreements_History");
                    temp.HasPeriodStart("SysStartTime");
                    temp.HasPeriodEnd("SysEndTime");
                }));

            builder.HasKey(s => s.SolutionId);

            builder.Property(s => s.SolutionId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(s => s.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(s => s.Solution)
                .WithOne(s => s.ServiceLevelAgreement)
                .HasForeignKey<ServiceLevelAgreements>(s => s.SolutionId)
                .HasConstraintName("FK_ServiceLevelAgreements_Solution");

            builder.HasOne(s => s.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(s => s.LastUpdatedBy)
                .HasConstraintName("FK_ServiceLevelAgreements_LastUpdatedBy");
        }
    }
}
