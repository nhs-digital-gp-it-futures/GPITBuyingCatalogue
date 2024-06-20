using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

internal class SolutionIntegrationEntityTypeConfiguration : IEntityTypeConfiguration<SolutionIntegration>
{
    public void Configure(EntityTypeBuilder<SolutionIntegration> builder)
    {
        builder.ToTable("SolutionIntegrations", Schemas.Catalogue);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);

        builder.Property(x => x.IntegratesWith).HasMaxLength(100);

        builder.HasOne(x => x.Solution)
            .WithMany(x => x.Integrations)
            .HasForeignKey(x => x.CatalogueItemId)
            .HasConstraintName("FK_SolutionIntegrations_Solution")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.IntegrationType)
            .WithMany()
            .HasForeignKey(x => x.IntegrationTypeId)
            .HasConstraintName("FK_SolutionIntegrations_IntegrationType")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
