using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class AdditionalServicesModel(
        CatalogueItem catalogueItem,
        List<CatalogueItem> additionalServices,
        CatalogueItemContentStatus contentStatus)
        : SolutionDisplayBaseModel(catalogueItem, contentStatus)
    {
        public override int Index => 4;

        public IList<CatalogueItem> Services { get; set; } = additionalServices;
    }
}
