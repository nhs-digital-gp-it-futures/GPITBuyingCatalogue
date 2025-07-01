using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class CompetitionSublocationEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionSublocation>
{
    public void Configure(EntityTypeBuilder<CompetitionSublocation> builder)
    {
        builder.ToTable("CompetitionSublocations", Schemas.Competitions);

        builder.HasKey(x => new { x.CompetitionId, x.SublocationOdsCode });

        builder.Property(x => x.CompetitionId).IsRequired();

        builder.Property(x => x.SublocationOdsCode).HasMaxLength(10).IsRequired();

        builder.Property(x => x.OwnerOdsCode).HasMaxLength(10).IsRequired();

        builder.HasOne(x => x.Competition)
            .WithMany(y => y.CompetitionSublocations)
            .HasForeignKey(x => x.CompetitionId)
            .HasConstraintName("FK_CompetitionSublocations_Competitions");

        builder.HasMany(x => x.SublocationRecipients)
            .WithOne(y => y.ParentSublocation)
            .HasForeignKey(y => new { y.CompetitionId, y.ParentSublocationOdsCode })
            .HasConstraintName("FK_CompetitionSublocationRecipients_CompetitionSublocations")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SublocationOrganisation)
            .WithMany()
            .HasForeignKey(x => x.SublocationOdsCode)
            .HasConstraintName("FK_CompetitionSublocations_OdsOrganisations_Sublocation");
    }
}
