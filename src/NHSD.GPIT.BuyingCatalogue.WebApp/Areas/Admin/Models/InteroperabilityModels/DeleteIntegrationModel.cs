using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    public class DeleteIntegrationModel : NavBaseModel
    {
        public DeleteIntegrationModel()
        {
        }

        public DeleteIntegrationModel(CatalogueItem solution)
            : this()
        {
            SolutionName = solution.Name;
        }

        public string SolutionName { get; }

        public int IntegrationId { get; init; }

        public string IntegrationType { get; init; }
    }
}
