using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class
    CompetitionAdditionalServiceEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionAdditionalService>
{
    public void Configure(EntityTypeBuilder<CompetitionAdditionalService> builder)
    {
        builder.Property(x => x.ParentItemId).HasColumnName(nameof(CompetitionAdditionalService.ParentItemId));
    }
}
