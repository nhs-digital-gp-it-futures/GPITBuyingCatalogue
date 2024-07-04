using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionCatalogueItemPrice : IPrice
{
    public CompetitionCatalogueItemPrice()
    {
        Tiers = new HashSet<CompetitionCatalogueItemPriceTier>();
    }

    public CompetitionCatalogueItemPrice(CataloguePrice cataloguePrice, int competitionId)
        : this()
    {
        CompetitionId = competitionId;
        CataloguePriceId = cataloguePrice.CataloguePriceId;
        ProvisioningType = cataloguePrice.ProvisioningType;
        CataloguePriceType = cataloguePrice.CataloguePriceType;
        CataloguePriceCalculationType = cataloguePrice.CataloguePriceCalculationType;
        BillingPeriod = cataloguePrice.TimeUnit;
        CataloguePriceQuantityCalculationType = cataloguePrice.CataloguePriceQuantityCalculationType;
        CurrencyCode = cataloguePrice.CurrencyCode;
        Description = cataloguePrice.PricingUnit.Description;
        RangeDescription = cataloguePrice.PricingUnit.RangeDescription;

        foreach (var tier in cataloguePrice.CataloguePriceTiers)
        {
            Tiers.Add(new(tier, competitionId));
        }
    }

    public int Id { get; set; }

    public int CompetitionId { get; set; }

    public int CataloguePriceId { get; set; }

    public TimeUnit? BillingPeriod { get; set; }

    public ProvisioningType ProvisioningType { get; set; }

    public CataloguePriceType CataloguePriceType { get; set; }

    public CataloguePriceCalculationType CataloguePriceCalculationType { get; set; }

    public CataloguePriceQuantityCalculationType? CataloguePriceQuantityCalculationType { get; set; }

    public string CurrencyCode { get; set; }

    public string Description { get; set; }

    public string RangeDescription { get; set; }

    public ICollection<IPriceTier> PriceTiers => Tiers.Cast<IPriceTier>().ToList();

    public ICollection<CompetitionCatalogueItemPriceTier> Tiers { get; set; }

    public Competition Competition { get; set; }
}
