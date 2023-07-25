using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class SolutionScoreEntityTypeConfiguration : IEntityTypeConfiguration<SolutionScore>
{
    public void Configure(EntityTypeBuilder<SolutionScore> builder)
    {
        builder.ToTable("SolutionScores", Schemas.Competitions);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SolutionId)
            .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

        builder.Property(x => x.ScoreType)
            .HasConversion<int>();
    }
}
