using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class SolutionQuantityEntityTypeConfiguration : IEntityTypeConfiguration<SolutionQuantity>
{
    public void Configure(EntityTypeBuilder<SolutionQuantity> builder)
    {
        builder.ToTable("SolutionQuantities", Schemas.Competitions);

        builder.HasKey(x => new { x.CompetitionId, x.SolutionId, x.OdsCode });

        builder.Property(x => x.CompetitionId).IsRequired();

        builder.Property(x => x.SolutionId).IsRequired();

        builder.Property(x => x.OdsCode).IsRequired();

        builder.Property(x => x.Quantity).IsRequired();

        builder.HasOne(x => x.CompetitionSolution)
            .WithMany(x => x.Quantities)
            .HasForeignKey(x => new { x.CompetitionId, x.SolutionId })
            .HasConstraintName("FK_SolutionQuantities_Solution")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CompetitionRecipient)
            .WithMany()
            .HasForeignKey(x => new { x.CompetitionId, x.OdsCode })
            .HasConstraintName("FK_SolutionQuantities_Recipient")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
