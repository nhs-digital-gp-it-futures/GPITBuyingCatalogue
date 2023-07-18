using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class
        FilterCapabilityEpicEntityTypeConfiguration : IEntityTypeConfiguration<FilterCapabilityEpic>
    {
        public void Configure(EntityTypeBuilder<FilterCapabilityEpic> builder)
        {
            builder.ToTable(
                "FilterCapabilityEpics",
                Schemas.Filtering);

            builder.HasKey(f => f.Id);
        }
    }
}
