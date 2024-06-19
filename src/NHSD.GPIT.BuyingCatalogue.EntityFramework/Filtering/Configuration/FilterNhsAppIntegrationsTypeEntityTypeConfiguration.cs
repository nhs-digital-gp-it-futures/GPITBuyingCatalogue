using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FilterNhsAppIntegrationsTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterNhsAppIntegrationsType>
    {
        public void Configure(EntityTypeBuilder<FilterNhsAppIntegrationsType> builder)
        {
            builder.ToTable(
                "FilterNhsAppIntegrationTypes",
                Schemas.Filtering);

            builder.HasKey(fht => fht.Id);

            builder.Property(fht => fht.NhsAppIntegrationsType)
                .HasConversion<int>()
                .HasColumnName("NhsAppIntegrationsTypeId");
        }
    }
}
