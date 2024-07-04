using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FilterHostingTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterHostingType>
    {
        public void Configure(EntityTypeBuilder<FilterHostingType> builder)
        {
            builder.ToTable(
                "FilterHostingTypes",
                Schemas.Filtering);

            builder.HasKey(fht => fht.Id);

            builder.Property(fht => fht.HostingType)
                .HasConversion<int>()
                .HasColumnName("HostingTypeId");
        }
    }
}
