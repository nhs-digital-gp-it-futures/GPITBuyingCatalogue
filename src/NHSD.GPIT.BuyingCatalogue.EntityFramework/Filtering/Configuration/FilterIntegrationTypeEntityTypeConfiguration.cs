using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration;

internal sealed class FilterIntegrationTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterIntegrationType>
{
    public void Configure(EntityTypeBuilder<FilterIntegrationType> builder)
    {
        builder.ToTable("FilterIntegrationTypes", Schemas.Filtering);

        builder.HasKey(x => new { x.FilterId, x.IntegrationId, x.IntegrationTypeId });

        builder.HasOne(x => x.Filter)
            .WithMany()
            .HasForeignKey(x => x.FilterId)
            .HasConstraintName("FK_FilterIntegrationTypes_Filter");

        builder.HasOne(x => x.Integration)
            .WithMany(x => x.IntegrationTypes)
            .HasForeignKey(x => new { x.FilterId, x.IntegrationId })
            .HasConstraintName("FK_FilterIntegrationTypes_Integration");

        builder.HasOne(x => x.IntegrationType)
            .WithMany()
            .HasForeignKey(x => x.IntegrationTypeId)
            .HasConstraintName("FK_FilterIntegrationTypes_IntegrationType");
    }
}
