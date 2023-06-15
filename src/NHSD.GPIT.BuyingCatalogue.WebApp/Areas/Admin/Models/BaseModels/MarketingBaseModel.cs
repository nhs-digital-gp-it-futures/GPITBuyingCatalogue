using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BaseModels
{
    public abstract class MarketingBaseModel : NavBaseModel
    {
        protected MarketingBaseModel(CatalogueItem catalogueItem)
        {
            CatalogueItem = catalogueItem;
            ClientApplication = CatalogueItem?.Solution?.GetApplicationTypes();
            SolutionId = CatalogueItem?.Id;
            SupplierId = CatalogueItem?.Supplier?.Id ?? 0;
        }

        public CatalogueItemId? SolutionId { get; set; }

        public int SupplierId { get; set; }

        public ApplicationTypes ClientApplication { get; set; }

        protected CatalogueItem CatalogueItem { get; set; }
    }
}
