using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class
        FilterClientApplicationTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterClientApplicationType>
    {
        public void Configure(EntityTypeBuilder<FilterClientApplicationType> builder)
        {
            builder.ToTable(
                "FilterClientApplicationTypes",
                Schemas.Filtering);

            builder.HasKey(fcat => fcat.Id);

            builder.Property(p => p.ClientApplicationType)
                .HasConversion<int>()
                .HasColumnName("ClientApplicationTypeId");
        }
    }
}
