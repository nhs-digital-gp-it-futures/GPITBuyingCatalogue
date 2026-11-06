using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public class CommunityPharmacyListPriceModel : CommunityPharmacySolutionDisplayBaseModel, IListPriceModel
{
    public CommunityPharmacyListPriceModel(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus, bool isSubPage = false)
        : base(catalogueItem, contentStatus, isSubPage)
    {
        FlatListPrices = catalogueItem.CataloguePrices
            .Where(p =>
                p.CataloguePriceType == CataloguePriceType.Flat
                && p.PublishedStatus == PublicationStatus.Published)
            .ToList();

        TieredListPrices = catalogueItem.CataloguePrices
            .Where(p =>
                p.CataloguePriceType == CataloguePriceType.Tiered
                && p.PublishedStatus == PublicationStatus.Published)
            .ToList();

        ItemType = catalogueItem.CatalogueItemType;
    }

    public override int Index => 1;

    public string PriceFor { get; set; } = "Catalogue solution";

    public CatalogueItemType ItemType { get; set; }

    public IList<CataloguePrice> FlatListPrices { get; set; }

    public IList<CataloguePrice> TieredListPrices { get; set; }

    public bool HasFlatListPrices() => FlatListPrices?.Any() == true;
}
