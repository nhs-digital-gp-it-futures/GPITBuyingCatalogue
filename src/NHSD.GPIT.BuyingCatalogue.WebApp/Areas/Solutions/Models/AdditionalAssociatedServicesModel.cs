using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

public sealed class AdditionalAssociatedServicesModel : AssociatedServicesModel
{
    public AdditionalAssociatedServicesModel(
        CatalogueItem catalogueItem,
        CatalogueItem additionalService,
        List<CatalogueItem> associatedServices,
        CatalogueItemContentStatus contentStatus)
        : base(catalogueItem, associatedServices, contentStatus)
    {
        Title = "Associated services";
        Caption = additionalService.Name;
        IsSubPage = true;
    }

    public override int Index => 4;
}
