using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DeleteApplicationTypeModels
{
    public class DeleteApplicationTypeConfirmationModel : NavBaseModel
    {
        public DeleteApplicationTypeConfirmationModel()
        {
        }

        public DeleteApplicationTypeConfirmationModel(CatalogueItemId solutionId, ClientApplicationType clientApplicationType)
        {
            BackLinkText = "Go back";

            if (clientApplicationType == ClientApplicationType.BrowserBased)
            {
                BackLink = $"/admin/catalogue-solutions/manage/{solutionId}/client-application-type/browser-based";
                ApplicationType = "browser-based";
            }
            else if (clientApplicationType == ClientApplicationType.Desktop)
            {
                BackLink = $"/admin/catalogue-solutions/manage/{solutionId}/client-application-type/desktop";
                ApplicationType = "desktop";
            }
            else
            {
                BackLink = $"/admin/catalogue-solutions/manage/{solutionId}/client-application-type/mobiletablet";
                ApplicationType = "mobile or tablet";
            }
        }

        public string ApplicationType { get; set; }
    }
}
