using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FilterInteropIntegrationTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterInteroperabilityIntegrationType>
    {
        public void Configure(EntityTypeBuilder<FilterInteroperabilityIntegrationType> builder)
        {
            builder.ToTable(
                "FilterInteroperabilityIntegrationTypes",
                Schemas.Filtering);

            builder.HasKey(fht => fht.Id);

            builder.Property(fht => fht.InteroperabilityIntegrationType)
                .HasConversion<int>()
                .HasColumnName("InteroperabilityIntegrationTypeId");
        }
    }
}
