using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class ImplementationCriteriaEntityTypeConfiguration : IEntityTypeConfiguration<ImplementationCriteria>
{
    public void Configure(EntityTypeBuilder<ImplementationCriteria> builder)
    {
        builder.ToTable("ImplementationCriteria", Schemas.Competitions);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Requirements).IsRequired().HasMaxLength(1100);
    }
}
