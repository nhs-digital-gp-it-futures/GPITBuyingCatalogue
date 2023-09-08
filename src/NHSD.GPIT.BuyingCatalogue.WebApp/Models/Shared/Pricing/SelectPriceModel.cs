using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;

public sealed class SelectPriceModel : NavBaseModel
{
    public SelectPriceModel()
    {
    }

    public SelectPriceModel(CatalogueItem solution)
    {
        SolutionName = solution.Name;
        SolutionType = solution.CatalogueItemType.Name();
        Prices = solution.CataloguePrices.OrderBy(cp => cp.CataloguePriceType).ToList();
    }

    public List<CataloguePrice> Prices { get; set; }

    public int? SelectedPriceId { get; set; }

    public string SolutionName { get; set; }

    public string SolutionType { get; set; }

    public RoutingSource? Source { get; set; }
}
