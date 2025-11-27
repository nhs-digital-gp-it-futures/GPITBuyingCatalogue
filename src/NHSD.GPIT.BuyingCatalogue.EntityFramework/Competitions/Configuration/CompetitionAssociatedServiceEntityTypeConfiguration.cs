using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class
    CompetitionAssociatedServiceEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionAssociatedService>
{
    public void Configure(EntityTypeBuilder<CompetitionAssociatedService> builder)
    {
        builder.Property(x => x.ParentItemId).HasColumnName(nameof(CompetitionAssociatedService.ParentItemId));
    }
}
