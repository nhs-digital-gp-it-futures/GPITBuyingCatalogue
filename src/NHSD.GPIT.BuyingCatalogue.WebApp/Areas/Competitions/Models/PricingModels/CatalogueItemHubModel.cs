using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

public class CatalogueItemHubModel
{
    public CatalogueItemHubModel()
    {
    }

    public CatalogueItemHubModel(
        CatalogueItemId solutionId,
        CatalogueItem catalogueItem,
        int? globalQuantity,
        Dictionary<OdsOrganisation, int?> recipientQuantities,
        CompetitionCatalogueItemPrice selectedPrice)
    {
        SolutionId = solutionId;
        CatalogueItemName = catalogueItem.Name;
        CatalogueItemId = catalogueItem.Id;
        CatalogueItemType = catalogueItem.CatalogueItemType;
        NumberOfCataloguePrices = catalogueItem.CataloguePrices.Count;
        OdsOrganisations = recipientQuantities;
        Price = selectedPrice;
        PriceId = PriceProgress is TaskProgress.Completed
            ? Price.CataloguePriceId
            : catalogueItem.CataloguePrices.First().CataloguePriceId;
        GlobalQuantity = globalQuantity;
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string CatalogueItemName { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public CatalogueItemId CatalogueItemId { get; set; }

    public CatalogueItemType CatalogueItemType { get; set; }

    public int NumberOfCataloguePrices { get; set; }

    public int PriceId { get; set; }

    public int? GlobalQuantity { get; set; }

    public int? ContractLength { get; set; }

    public Dictionary<OdsOrganisation, int?> OdsOrganisations { get; set; }

    public CompetitionCatalogueItemPrice Price { get; set; }

    public TaskProgress PriceProgress => Price?.Tiers?.Count > 0
        ? TaskProgress.Completed
        : TaskProgress.NotStarted;

    public TaskProgress QuantityProgress
    {
        get
        {
            if (PriceProgress is TaskProgress.NotStarted)
                return TaskProgress.CannotStart;

            return GlobalQuantity.HasValue || OdsOrganisations.All(x => x.Value.HasValue)
                ? TaskProgress.Completed
                : OdsOrganisations.All(x => !x.Value.HasValue)
                    ? TaskProgress.NotStarted
                    : TaskProgress.InProgress;
        }
    }
}
