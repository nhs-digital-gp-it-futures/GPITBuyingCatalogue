using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FilterIM1IntegrationsTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterIM1IntegrationsType>
    {
        public void Configure(EntityTypeBuilder<FilterIM1IntegrationsType> builder)
        {
            builder.ToTable(
                "FilterIM1IntegrationsTypes",
                Schemas.Filtering);

            builder.HasKey(fht => fht.Id);

            builder.Property(fht => fht.IM1IntegrationsType)
                .HasConversion<int>()
                .HasColumnName("IM1IntegrationsTypeId");
        }
    }
}
