using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration
{
    public sealed class CompetitionRecipientEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionRecipient>
    {
        public void Configure(EntityTypeBuilder<CompetitionRecipient> builder)
        {
            builder.ToTable("CompetitionRecipients", Schemas.Competitions);

            builder.HasKey(x => new { x.CompetitionId, x.OdsCode });

            builder.Property(x => x.CompetitionId).IsRequired();
            builder.Property(x => x.OdsCode).IsRequired();
        }
    }
}
