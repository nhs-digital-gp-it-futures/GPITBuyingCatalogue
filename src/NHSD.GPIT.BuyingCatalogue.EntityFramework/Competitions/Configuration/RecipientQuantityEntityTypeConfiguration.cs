using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class RecipientQuantityEntityTypeConfiguration : IEntityTypeConfiguration<RecipientQuantity>
{
    public void Configure(EntityTypeBuilder<RecipientQuantity> builder)
    {
        builder.ToTable("RecipientQuantities", Schemas.Competitions);

        builder.HasKey(x => new { x.CompetitionId, x.OdsCode, x.CatalogueItemId });

        builder.Property(x => x.CatalogueItemId)
            .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));
    }
}
