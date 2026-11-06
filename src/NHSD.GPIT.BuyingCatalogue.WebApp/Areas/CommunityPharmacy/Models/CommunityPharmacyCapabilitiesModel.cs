using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public class CommunityPharmacyCapabilitiesModel : CommunityPharmacySolutionDisplayBaseModel, ICapabilitiesModel
{
    public CommunityPharmacyCapabilitiesModel(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus, bool isSubPage = false)
        : base(catalogueItem, contentStatus, isSubPage)
    {
        RowViewModels = catalogueItem.CatalogueItemCapabilities
            .Where(cic => cic.Capability.Status != CapabilityStatus.Expired)
            .Select(cic => new RowViewModel(cic)).ToList();
    }

    public override int Index => 2;

    public string Name { get; set; }

    public string Description { get; set; }

    public IList<RowViewModel> RowViewModels { get; }

    public string CapabilitiesHeading { get; set; } = "Capabilities met";
}
