using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class
    CompetitionRecipientCommissionedByEntityTypeConfiguration : IEntityTypeConfiguration<
    CompetitionRecipientCommissionedBy>
{
    public void Configure(EntityTypeBuilder<CompetitionRecipientCommissionedBy> builder)
    {
        builder.ToTable("CompetitionRecipientCommissionedBy", Schemas.Competitions);

        builder.HasKey(x => new { x.CompetitionId, x.RecipientOdsCode });

        builder.Property(x => x.CompetitionId).IsRequired();

        builder.Property(x => x.CommissionedByOdsCode).IsRequired();

        builder.HasOne(x => x.Competition)
            .WithMany()
            .HasForeignKey(x => x.CompetitionId)
            .HasConstraintName("FK_CompetitionRecipientCommissionedBy_Competitions")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CompetitionRecipient)
            .WithMany()
            .HasForeignKey(x => new { x.CompetitionId, x.RecipientOdsCode })
            .HasConstraintName("FK_CompetitionRecipientCommissionedBy_CompetitionRecipients")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CommissionedByCompetitionSublocations)
            .WithMany()
            .HasForeignKey(x => new { x.CompetitionId, x.CommissionedByOdsCode })
            .HasConstraintName("FK_CompetitionRecipientCommissionedBy_CompetitionSublocations")
            .OnDelete(DeleteBehavior.Cascade);

        // TODO: Add OdsOrganisation entities for recipient and sublocation
    }
}
