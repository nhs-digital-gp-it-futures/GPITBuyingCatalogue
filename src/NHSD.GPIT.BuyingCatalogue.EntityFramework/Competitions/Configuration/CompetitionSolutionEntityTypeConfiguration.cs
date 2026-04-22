using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class CompetitionSolutionEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionSolution>
{
    public void Configure(EntityTypeBuilder<CompetitionSolution> builder)
    {
        builder.Property(x => x.Justification).HasMaxLength(1000);

        builder.HasMany(x => x.Services)
            .WithOne()
            .HasForeignKey(x => x.ParentItemId)
            .HasConstraintName("FK_CompetitionCatalogueItems_Parent")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Scores)
            .WithOne()
            .HasForeignKey(x => x.CompetitionSolutionId)
            .HasConstraintName("FK_SolutionScores_Solution")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
