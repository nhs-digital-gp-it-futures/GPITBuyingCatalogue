using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class
    ServiceQuantitySublocationRecipientEntityTypeConfiguration : IEntityTypeConfiguration<
    ServiceQuantitySublocationRecipient>
{
    public void Configure(EntityTypeBuilder<ServiceQuantitySublocationRecipient> builder)
    {
        builder.ToTable("ServiceQuantitiesSublocationRecipients", Schemas.Competitions);

        builder.HasKey(x => new { x.CompetitionId, x.SolutionId, x.ServiceId, x.OdsCode });

        builder.Property(x => x.CompetitionId).IsRequired();

        builder.Property(x => x.SolutionId).IsRequired();

        builder.Property(x => x.OdsCode).IsRequired();

        builder.Property(x => x.Quantity).IsRequired();

        builder.HasOne(x => x.SolutionService)
            .WithMany(x => x.Quantities)
            .HasForeignKey(x => new { x.CompetitionId, x.SolutionId, x.ServiceId })
            .HasConstraintName("FK_ServiceQuantitiesSublocationRecipients_Solution")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CompetitionSublocationRecipient)
            .WithMany()
            .HasForeignKey(x => new { x.CompetitionId, x.OdsCode })
            .HasConstraintName("FK_ServiceQuantitiesSublocationRecipients_SublocationRecipient")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
