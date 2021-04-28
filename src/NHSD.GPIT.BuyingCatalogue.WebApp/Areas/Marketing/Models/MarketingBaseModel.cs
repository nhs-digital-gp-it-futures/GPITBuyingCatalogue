using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public abstract class MarketingBaseModel : NavBaseModel
    {
        protected MarketingBaseModel(CatalogueItem catalogueItem)
        {
            BackLinkText = "Return to all sections";
            CatalogueItem = catalogueItem;
            ClientApplication = CatalogueItem?.Solution?.GetClientApplication();
            SolutionId = CatalogueItem?.CatalogueItemId;
            SupplierId = CatalogueItem?.Supplier?.Id;
        }

        public abstract bool? IsComplete { get; }

        protected CatalogueItem CatalogueItem { get; set; }

        public string SolutionId { get; set; }

        public string SupplierId { get; set; }

        public ClientApplication ClientApplication { get; set; }

        protected string GetStatus(MarketingBaseModel model) =>
            model.IsComplete.GetValueOrDefault() ? "COMPLETE" : "INCOMPLETE";
    }
}
