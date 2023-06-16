using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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
            ApplicationTypes = catalogueItem?.Solution?.GetApplicationTypes() ?? new ApplicationTypes();
            SolutionId = catalogueItem?.Id;
            SolutionName = catalogueItem?.Name;
        }

        public string SolutionName { get; set; }

        public ApplicationTypes ApplicationTypes { get; set; }

        public CatalogueItemId? SolutionId { get; set; }

        public ApplicationType ApplicationType { get; set; }
    }
}
