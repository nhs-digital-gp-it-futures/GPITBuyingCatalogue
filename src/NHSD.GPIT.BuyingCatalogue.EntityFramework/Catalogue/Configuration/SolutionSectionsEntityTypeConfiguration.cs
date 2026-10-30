using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

internal sealed class SolutionSectionsEntityTypeConfiguration : IEntityTypeConfiguration<SolutionSections>
{
    public void Configure(EntityTypeBuilder<SolutionSections> builder)
    {
        builder.ToTable("SolutionSections", Schemas.Catalogue);

        builder.HasKey(i => i.SolutionType);

        builder.Property(i => i.SolutionType)
            .HasConversion<int>()
            .HasColumnName("SolutionTypeId");

        builder.Property(i => i.Details)
            .HasConversion<int>()
            .HasColumnName("Details");

        builder.Property(i => i.Description)
            .HasConversion<int>()
            .HasColumnName("Description");

        builder.Property(i => i.Features)
            .HasConversion<int>()
            .HasColumnName("Features");

        builder.Property(i => i.Integrations)
            .HasConversion<int>()
            .HasColumnName("Integrations");

        builder.Property(i => i.Implementation)
            .HasConversion<int>()
            .HasColumnName("Implementation");

        builder.Property(i => i.ApplicationType)
            .HasConversion<int>()
            .HasColumnName("ApplicationType");

        builder.Property(i => i.HostingType)
            .HasConversion<int>()
            .HasColumnName("HostingType");

        builder.Property(i => i.DataProcessingInformation)
            .HasConversion<int>()
            .HasColumnName("DataProcessingInformation");

        builder.Property(i => i.ListPrice)
            .HasConversion<int>()
            .HasColumnName("ListPrice");

        builder.Property(i => i.CapabilitiesAndEpics)
            .HasConversion<int>()
            .HasColumnName("CapabilitiesAndEpics");

        builder.Property(i => i.AdditionalServices)
            .HasConversion<int>()
            .HasColumnName("AdditionalServices");

        builder.Property(i => i.AssociatedServices)
            .HasConversion<int>()
            .HasColumnName("AssociatedServices");

        builder.Property(i => i.Standards)
            .HasConversion<int>()
            .HasColumnName("Standards");

        builder.Property(i => i.DevelopmentPlans)
            .HasConversion<int>()
            .HasColumnName("DevelopmentPlans");

        builder.Property(i => i.SupplierDetails)
            .HasConversion<int>()
            .HasColumnName("SupplierDetails");

        builder.Property(i => i.ServiceLevelAgreement)
            .HasConversion<int>()
            .HasColumnName("ServiceLevelAgreement");
    }
}
