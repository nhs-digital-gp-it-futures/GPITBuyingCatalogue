using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class CompetitionWeightingsEntityTypeConfiguration : IEntityTypeConfiguration<Weightings>
{
    public void Configure(EntityTypeBuilder<Weightings> builder)
    {
        builder.ToTable("Weightings", Schemas.Competitions);

        builder.HasKey(x => x.CompetitionId);
    }
}
