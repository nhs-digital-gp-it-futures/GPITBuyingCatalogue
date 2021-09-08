using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public abstract class MarketingBaseModel : NavBaseModel
    {
        protected MarketingBaseModel(CatalogueItem catalogueItem)
        {
            BackLinkText = "Go back";
            CatalogueItem = catalogueItem;
            ClientApplication = CatalogueItem?.Solution?.GetClientApplication();
            SolutionId = CatalogueItem?.Id;
            SupplierId = CatalogueItem?.Supplier?.Id ?? 0;
        }

        public abstract bool? IsComplete { get; }

        public CatalogueItemId? SolutionId { get; set; }

        public int SupplierId { get; set; }

        public ClientApplication ClientApplication { get; set; }

        protected CatalogueItem CatalogueItem { get; set; }

        protected string GetStatus(MarketingBaseModel model) =>
            model.IsComplete.GetValueOrDefault() ? "COMPLETE" : "INCOMPLETE";
    }
}
