using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
        CatalogueItem catalogueItem,
        Dictionary<OdsOrganisation, int?> recipientQuantities)
    {
        CatalogueItemName = catalogueItem.Name;
        CatalogueItemId = catalogueItem.Id;
        CatalogueItemType = catalogueItem.CatalogueItemType;
        OdsOrganisations = recipientQuantities;
    }

    public string CatalogueItemName { get; set; }

    public CatalogueItemId CatalogueItemId { get; set; }

    public CatalogueItemType CatalogueItemType { get; set; }

    public Dictionary<OdsOrganisation, int?> OdsOrganisations { get; set; }

    public TaskProgress PriceProgress => TaskProgress.NotStarted;

    public TaskProgress QuantityProgress => !OdsOrganisations.All(x => x.Value.HasValue)
        ? TaskProgress.NotStarted
        : OdsOrganisations.Any(x => !x.Value.HasValue)
            ? TaskProgress.InProgress
            : TaskProgress.Completed;
}
