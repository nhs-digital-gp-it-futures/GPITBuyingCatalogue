using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration;

internal sealed class FilterIntegrationEntityTypeConfiguration : IEntityTypeConfiguration<FilterIntegration>
{
    public void Configure(EntityTypeBuilder<FilterIntegration> builder)
    {
        builder.ToTable("FilterIntegrations", Schemas.Filtering);

        builder.HasKey(x => new { x.FilterId, x.IntegrationId });

        builder.HasOne(x => x.Filter)
            .WithMany(x => x.Integrations)
            .HasForeignKey(x => x.FilterId)
            .HasConstraintName("FK_FilterIntegrations_Filter");

        builder.HasOne(x => x.Integration)
            .WithMany()
            .HasForeignKey(x => x.IntegrationId)
            .HasConstraintName("FK_FilterIntegrations_Integration");
    }
}
