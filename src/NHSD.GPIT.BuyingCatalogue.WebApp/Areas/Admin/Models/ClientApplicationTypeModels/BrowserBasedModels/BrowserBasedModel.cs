using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels
{
    public class BrowserBasedModel : ApplicationTypeBaseModel
    {
        public BrowserBasedModel()
            : base(null)
        {
        }

        public BrowserBasedModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            ApplicationType = ApplicationType.BrowserBased;
        }
    }
}
