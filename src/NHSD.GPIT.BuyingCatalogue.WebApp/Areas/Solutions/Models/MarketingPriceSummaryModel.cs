using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

[ExcludeFromCodeCoverage]
public class MarketingPriceSummaryModel
{
    public CatalogueItemId SolutionId { get; set; }

    public CatalogueItemId ServiceId { get; set; }

    public string CapabilitiesAndEpicsUrl { get; set; }

    public string PricePageUrl { get; set; }

    public string OrderGuidance { get; set; }

    public string ItemDescription { get; set; }

    public IEnumerable<CataloguePrice> Prices { get; set; }
}
