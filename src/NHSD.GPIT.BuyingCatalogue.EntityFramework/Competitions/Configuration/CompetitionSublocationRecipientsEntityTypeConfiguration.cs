using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class
    CompetitionSublocationRecipientsEntityTypeConfiguration : IEntityTypeConfiguration<
    CompetitionSublocationRecipient>
{
    public void Configure(EntityTypeBuilder<CompetitionSublocationRecipient> builder)
    {
        builder.ToTable("CompetitionSublocationRecipients", Schemas.Competitions);

        builder.HasKey(x => new { x.CompetitionId, x.RecipientOdsCode });

        builder.Property(x => x.CompetitionId).IsRequired();

        builder.Property(x => x.RecipientOdsCode).IsRequired();

        builder.Property(x => x.ParentSublocationOdsCode).IsRequired();

        builder.HasOne(x => x.Competition)
            .WithMany()
            .HasForeignKey(x => x.CompetitionId)
            .HasConstraintName("FK_CompetitionSublocationRecipients_Competitions")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ParentSublocation)
            .WithMany()
            .HasForeignKey(x => new { x.CompetitionId, x.ParentSublocationOdsCode })
            .HasConstraintName("FK_CompetitionSublocationRecipients_CompetitionSublocations")
            .OnDelete(DeleteBehavior.Cascade);

        // TODO: Add OdsOrganisation entities for recipient and sublocation (if required)
    }
}
