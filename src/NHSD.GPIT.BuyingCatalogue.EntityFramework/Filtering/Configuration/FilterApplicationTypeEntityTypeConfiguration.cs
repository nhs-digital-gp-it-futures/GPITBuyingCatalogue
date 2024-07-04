using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class
        FilterApplicationTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterApplicationType>
    {
        public void Configure(EntityTypeBuilder<FilterApplicationType> builder)
        {
            builder.ToTable(
                "FilterClientApplicationTypes",
                Schemas.Filtering);

            builder.HasKey(fcat => fcat.Id);

            builder.Property(p => p.ApplicationTypeID)
                .HasConversion<int>()
                .HasColumnName("ClientApplicationTypeId");
        }
    }
}
