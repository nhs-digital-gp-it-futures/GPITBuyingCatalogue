using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public interface IListPriceModel
{
    string PriceFor { get; }

    CatalogueItemType ItemType { get; }

    IList<CataloguePrice> FlatListPrices { get; }

    IList<CataloguePrice> TieredListPrices { get; }

    bool IsSubPage { get; }

    string BackLink { get; }

    bool HasFlatListPrices();
}
