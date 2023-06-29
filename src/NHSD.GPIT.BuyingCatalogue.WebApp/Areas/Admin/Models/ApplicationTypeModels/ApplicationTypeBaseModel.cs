using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels
{
    public class ApplicationTypeBaseModel : NavBaseModel
    {
        public ApplicationTypeBaseModel()
        {
        }

        public ApplicationTypeBaseModel(CatalogueItem catalogueItem)
        {
            ApplicationTypeDetail = catalogueItem?.Solution?.EnsureAndGetApplicationType() ?? new ApplicationTypeDetail();
            SolutionId = catalogueItem?.Id;
            SolutionName = catalogueItem?.Name;
        }

        public string SolutionName { get; set; }

        public ApplicationTypeDetail ApplicationTypeDetail { get; set; }

        public ApplicationTypeProgress ApplicationTypeProgress => new ApplicationTypeProgress(ApplicationTypeDetail);

        public CatalogueItemId? SolutionId { get; set; }

        public ApplicationType ApplicationType { get; set; }
    }
}
