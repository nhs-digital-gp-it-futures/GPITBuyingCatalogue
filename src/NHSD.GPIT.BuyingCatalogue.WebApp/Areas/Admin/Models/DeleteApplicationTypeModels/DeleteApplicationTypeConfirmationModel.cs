using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DeleteApplicationTypeModels
{
    public class DeleteApplicationTypeConfirmationModel : NavBaseModel
    {
        public DeleteApplicationTypeConfirmationModel()
        {
        }

        public DeleteApplicationTypeConfirmationModel(CatalogueItem solution, ClientApplicationType clientApplicationType)
        {
            BackLinkText = "Go back";

            if (clientApplicationType == ClientApplicationType.BrowserBased)
            {
                BackLink = $"/admin/catalogue-solutions/manage/{solution.Id}/client-application-type/browser-based";
                ApplicationType = "browser-based";
            }
            else if (clientApplicationType == ClientApplicationType.Desktop)
            {
                BackLink = $"/admin/catalogue-solutions/manage/{solution.Id}/client-application-type/desktop";
                ApplicationType = "desktop";
            }
            else
            {
                BackLink = $"/admin/catalogue-solutions/manage/{solution.Id}/client-application-type/mobiletablet";
                ApplicationType = "mobile or tablet";
            }

            SolutionName = solution.Name;
        }

        public string SolutionName { get; }

        public string ApplicationType { get; set; }
    }
}
