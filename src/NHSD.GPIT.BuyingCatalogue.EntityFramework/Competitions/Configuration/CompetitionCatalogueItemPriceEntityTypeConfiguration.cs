using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class CompetitionCatalogueItemPriceEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionCatalogueItemPrice>
{
    public void Configure(EntityTypeBuilder<CompetitionCatalogueItemPrice> builder)
    {
        builder.ToTable("CompetitionCatalogueItemPrices", Schemas.Competitions);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CataloguePriceId).IsRequired();

        builder.Property(x => x.BillingPeriod)
            .HasConversion<int>()
            .HasColumnName("BillingPeriodId");

        builder.Property(x => x.ProvisioningType)
            .HasConversion<int>()
            .HasColumnName("ProvisioningTypeId");

        builder.Property(x => x.CataloguePriceType)
            .HasConversion<int>()
            .HasColumnName("CataloguePriceTypeId");

        builder.Property(x => x.CataloguePriceCalculationType)
            .HasConversion<int>()
            .HasColumnName("CataloguePriceCalculationTypeId");

        builder.Property(x => x.CataloguePriceQuantityCalculationType)
            .HasConversion<int>()
            .HasColumnName("CataloguePriceQuantityCalculationTypeId");
    }
}
