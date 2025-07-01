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

        builder.Property(x => x.RecipientOdsCode).HasMaxLength(10).IsRequired();

        builder.Property(x => x.ParentSublocationOdsCode).HasMaxLength(10).IsRequired();

        builder.HasOne(x => x.Competition)
            .WithMany()
            .HasForeignKey(x => x.CompetitionId)
            .HasConstraintName("FK_CompetitionSublocationRecipients_Competitions")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ParentSublocation)
            .WithMany(y => y.SublocationRecipients)
            .HasForeignKey(x => new { x.CompetitionId, x.ParentSublocationOdsCode })
            .HasConstraintName("FK_CompetitionSublocationRecipients_CompetitionSublocations")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RecipientOrganisation)
            .WithMany()
            .HasForeignKey(x => x.RecipientOdsCode)
            .HasConstraintName("FK_CompetitionSublocationRecipients_OdsOrganisations_Recipient");
    }
}
