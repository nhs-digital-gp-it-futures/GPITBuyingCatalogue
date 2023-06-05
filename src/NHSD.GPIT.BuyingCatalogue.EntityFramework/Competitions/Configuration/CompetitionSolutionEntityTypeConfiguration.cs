using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class CompetitionSolutionEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionSolution>
{
    public void Configure(EntityTypeBuilder<CompetitionSolution> builder)
    {
        builder.ToTable("Solutions", Schemas.Competitions);

        builder.HasKey(x => new { x.CompetitionId, x.SolutionId });

        builder.Property(x => x.SolutionId)
            .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

        builder.Property(x => x.IsShortlisted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne<Competition>()
            .WithMany(x => x.CompetitionSolutions)
            .HasForeignKey(x => x.CompetitionId)
            .HasConstraintName("FK_CompetitionSolutions_Competition");

        builder.HasOne(x => x.Solution)
            .WithMany()
            .HasForeignKey(x => x.SolutionId)
            .HasConstraintName("FK_CompetitionSolutions_Solution");
    }
}
