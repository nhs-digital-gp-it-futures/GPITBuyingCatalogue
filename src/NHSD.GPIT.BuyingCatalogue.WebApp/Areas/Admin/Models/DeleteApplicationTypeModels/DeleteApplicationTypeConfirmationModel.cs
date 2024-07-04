using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DeleteApplicationTypeModels
{
    public class DeleteApplicationTypeConfirmationModel : NavBaseModel
    {
        public DeleteApplicationTypeConfirmationModel()
        {
        }

        public DeleteApplicationTypeConfirmationModel(CatalogueItem solution, ApplicationType applicationType)
        {
            ApplicationType = applicationType
                .AsString(EnumFormat.DisplayName)
                .ToLowerInvariant();
            SolutionName = solution.Name;
        }

        public string SolutionName { get; }

        public string ApplicationType { get; set; }
    }
}
