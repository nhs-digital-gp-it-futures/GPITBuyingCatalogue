using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

internal sealed class IntegrationTypeEntityTypeConfiguration : IEntityTypeConfiguration<IntegrationType>
{
    public void Configure(EntityTypeBuilder<IntegrationType> builder)
    {
        builder.ToTable("IntegrationTypes", Schemas.Catalogue);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);

        builder.HasMany(x => x.Solutions)
            .WithMany()
            .UsingEntity<SolutionIntegration>(
                r => r.HasOne(x => x.Solution)
                    .WithMany(x => x.Integrations)
                    .HasForeignKey(nameof(SolutionIntegration.CatalogueItemId))
                    .HasConstraintName("FK_SolutionIntegrations_Solution"),
                l => l.HasOne(x => x.IntegrationType)
                    .WithMany()
                    .HasForeignKey(nameof(SolutionIntegration.IntegrationTypeId))
                    .HasConstraintName("FK_SolutionIntegration_IntegrationType"));

        builder.HasOne(x => x.Integration)
            .WithMany(x => x.IntegrationTypes)
            .HasForeignKey(x => x.IntegrationId)
            .HasConstraintName("FK_IntegrationTypes_Integration")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
