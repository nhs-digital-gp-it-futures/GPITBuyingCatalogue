using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public class CommunityPharmacyListPriceModel : CommunityPharmacySolutionDisplayBaseModel
{
    public CommunityPharmacyListPriceModel(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus, bool isSubPage = false)
        : base(catalogueItem, contentStatus, isSubPage)
    {
        ListPrices = catalogueItem.CataloguePrices
            .Where(p =>
                p.CataloguePriceType == CataloguePriceType.Flat
                && p.PublishedStatus == PublicationStatus.Published)
            .ToList();
    }

    public override int Index => 1;

    public IList<CataloguePrice> ListPrices { get; set; }
}
