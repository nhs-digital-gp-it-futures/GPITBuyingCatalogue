using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionCatalogueItemPriceTier : IPriceTier
{
    public CompetitionCatalogueItemPriceTier()
    {
    }

    public CompetitionCatalogueItemPriceTier(IPriceTier tier, int competitionId)
    {
        CompetitionId = competitionId;
        Price = tier.Price;
        ListPrice = tier.Price;
        LowerRange = tier.LowerRange;
        UpperRange = tier.UpperRange;
    }

    public int Id { get; set; }

    public int CompetitionItemPriceId { get; set; }

    public int CompetitionId { get; set; }

    public int LowerRange { get; set; }

    public int? UpperRange { get; set; }

    public decimal Price { get; set; }

    public decimal ListPrice { get; set; }

    public CompetitionCatalogueItemPrice CompetitionCatalogueItemPrice { get; set; }
}
