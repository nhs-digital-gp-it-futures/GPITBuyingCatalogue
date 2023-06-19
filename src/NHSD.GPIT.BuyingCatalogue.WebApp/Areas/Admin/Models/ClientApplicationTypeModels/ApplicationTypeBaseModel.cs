using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
            ClientApplication = catalogueItem?.Solution?.EnsureAndGetApplicationType() ?? new ApplicationTypeDetail();
            SolutionId = catalogueItem?.Id;
            SolutionName = catalogueItem?.Name;
        }

        public string SolutionName { get; set; }

        public ApplicationTypeDetail ClientApplication { get; set; }

        public ApplicationTypeProgress ClientApplicationProgress => new ApplicationTypeProgress(ClientApplication);

        public CatalogueItemId? SolutionId { get; set; }

        public ApplicationType ApplicationType { get; set; }
    }
}
