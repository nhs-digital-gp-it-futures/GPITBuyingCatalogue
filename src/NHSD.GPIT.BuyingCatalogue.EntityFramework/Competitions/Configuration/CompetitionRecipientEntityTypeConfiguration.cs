using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;
//
// public class CompetitionRecipientEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionRecipient>
// {
//     public void Configure(EntityTypeBuilder<CompetitionRecipient> builder)
//     {
//         builder.ToTable("CompetitionRecipients", Schemas.Competitions);
//
//         builder.HasKey(x => new { x.OdsCode, x.CompetitionId });
//

//
//         builder.HasMany(x => x.Quantities)
//             .WithOne()
//             .HasForeignKey(x => new { x.OdsCode, x.CompetitionId });
//     }
// }
