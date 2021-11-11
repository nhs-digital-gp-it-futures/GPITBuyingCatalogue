using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.HostingTypeModels
{
    public sealed class DeleteHostingTypeConfirmationModel : NavBaseModel
    {
        public DeleteHostingTypeConfirmationModel()
        {
        }

        public DeleteHostingTypeConfirmationModel(
            CatalogueItem solution,
            HostingType hostingType)
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;
            HostingType = hostingType;
        }

        public HostingType HostingType { get; init; }

        public CatalogueItemId SolutionId { get; init; }

        public string SolutionName { get; init; }
    }
}
