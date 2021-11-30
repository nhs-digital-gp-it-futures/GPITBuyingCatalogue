using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class AdditionalServicesModel : SolutionDisplayBaseModel
    {
        public AdditionalServicesModel(CatalogueItem catalogueItem, List<CatalogueItem> additionalServices)
            : base(catalogueItem) =>
            Services = additionalServices;

        public override int Index => 5;

        public IList<CatalogueItem> Services { get; set; }
    }
}
