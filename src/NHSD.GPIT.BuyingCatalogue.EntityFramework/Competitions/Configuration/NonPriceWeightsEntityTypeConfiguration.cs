using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class NonPriceWeightsEntityTypeConfiguration : IEntityTypeConfiguration<NonPriceWeights>
{
    public void Configure(EntityTypeBuilder<NonPriceWeights> builder)
    {
        builder.ToTable("NonPriceWeights", Schemas.Competitions);

        builder.HasKey(x => x.NonPriceElementsId);
    }
}
