using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels
{
    public class ApplicationTypeBaseModel : NavBaseModel
    {
        public ApplicationTypeBaseModel()
        {
        }

        public ApplicationTypeBaseModel(CatalogueItem catalogueItem)
        {
            ClientApplication = catalogueItem?.Solution?.GetClientApplication() ?? new ClientApplication();
            SolutionId = catalogueItem?.Id;
            SolutionName = catalogueItem?.Name;
        }

        public string SolutionName { get; set; }

        public ClientApplication ClientApplication { get; set; }

        public ClientApplicationProgress ClientApplicationProgress => new ClientApplicationProgress(ClientApplication);

        public CatalogueItemId? SolutionId { get; set; }

        public ClientApplicationType ApplicationType { get; set; }
    }
}
