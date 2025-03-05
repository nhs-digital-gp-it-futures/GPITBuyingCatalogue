using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class CompetitionSublocationEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionSublocation>
{
    public void Configure(EntityTypeBuilder<CompetitionSublocation> builder)
    {
        builder.ToTable("CompetitionSublocation", Schemas.Competitions);

        builder.HasKey(x => new { x.CompetitionId, x.SublocationOdsCode });

        builder.Property(x => x.CompetitionId).IsRequired();

        builder.Property(x => x.SublocationOdsCode).IsRequired();

        builder.Property(x => x.OwnerOdsCode).IsRequired();

        builder.Property(x => x.IsActive).HasConversion(v => v ? 1 : 0, v => v == 1).IsRequired();

        builder.HasMany(x => x.SublocationRecipients)
            .WithOne(y => y.ParentSublocation)
            .HasForeignKey(y => new { y.CompetitionId, y.ParentSublocationOdsCode })
            .HasConstraintName("FK_CompetitionSublocationRecipients_CompetitionSublocations");
    }
}
