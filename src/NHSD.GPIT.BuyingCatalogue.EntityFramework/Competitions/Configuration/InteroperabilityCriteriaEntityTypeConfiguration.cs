using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class InteroperabilityCriteriaEntityTypeConfiguration : IEntityTypeConfiguration<InteroperabilityCriteria>
{
    public void Configure(EntityTypeBuilder<InteroperabilityCriteria> builder)
    {
        builder.ToTable("InteroperabilityCriteria", Schemas.Competitions);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Qualifier).IsRequired();
    }
}
