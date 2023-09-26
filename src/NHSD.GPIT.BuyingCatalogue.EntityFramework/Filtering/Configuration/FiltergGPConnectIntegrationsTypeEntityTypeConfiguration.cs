using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FiltergGPConnectIntegrationsTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterGPConnectIntegrationsType>
    {
        public void Configure(EntityTypeBuilder<FilterGPConnectIntegrationsType> builder)
        {
            builder.ToTable(
                "FilterGPConnectIntegrationTypes",
                Schemas.Filtering);

            builder.HasKey(fht => fht.Id);

            builder.Property(fht => fht.GPConnectIntegrationsType)
                .HasConversion<int>()
                .HasColumnName("GPConnectIntegrationsTypeId");
        }
    }
}
