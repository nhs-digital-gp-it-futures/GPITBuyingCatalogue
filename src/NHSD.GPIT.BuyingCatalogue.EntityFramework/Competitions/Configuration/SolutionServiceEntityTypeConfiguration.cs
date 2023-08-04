using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class SolutionServiceEntityTypeConfiguration : IEntityTypeConfiguration<SolutionService>
{
    public void Configure(EntityTypeBuilder<SolutionService> builder)
    {
        builder.ToTable("SolutionServices", Schemas.Competitions);

        builder.HasKey(x => new { x.CompetitionId, x.SolutionId, x.ServiceId });

        builder.Property(x => x.SolutionId)
            .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

        builder.Property(x => x.ServiceId)
            .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

        builder.HasOne<Competition>()
            .WithMany()
            .HasForeignKey(x => x.CompetitionId)
            .HasConstraintName("FK_SolutionServices_Competition")
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne<CompetitionSolution>()
            .WithMany()
            .HasForeignKey(x => x.SolutionId)
            .HasPrincipalKey(x => x.SolutionId)
            .HasConstraintName("FK_SolutionServices_Solution")
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(x => x.Service)
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("FK_SolutionServices_Service")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
