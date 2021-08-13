using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class ApplicationTypeBaseModel : NavBaseModel
    {
        public ApplicationTypeBaseModel()
        {
        }

        public ApplicationTypeBaseModel(CatalogueItem catalogueItem)
        {
            BackLink = "./";
            BackLinkText = "Go back";
            ClientApplication = catalogueItem?.Solution?.GetClientApplication();
            SolutionId = catalogueItem?.Id;
        }

        public ClientApplication ClientApplication { get; set; }

        public virtual bool IsComplete { get; set; }

        public CatalogueItemId? SolutionId { get; set; }
    }
}
