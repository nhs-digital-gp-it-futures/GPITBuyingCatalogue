using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class AdditionalServicesModel : SolutionDisplayBaseModel
    {
        public AdditionalServicesModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            Services = catalogueItem.Supplier.CatalogueItems
                .Where(ci => ci.CatalogueItemType == CatalogueItemType.AdditionalService)
                .OrderBy(ci => ci.Name)
                .Select(ci => new AdditionalServiceModel(ci))
                .ToList();
        }

        public override int Index => 4;

        public IList<AdditionalServiceModel> Services { get; set; }
    }
}
