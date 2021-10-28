using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements
{
    public class DeleteServiceAvailabilityTimesModel : NavBaseModel
    {
        public DeleteServiceAvailabilityTimesModel()
        {
            BackLinkText = "Go back";
        }

        public DeleteServiceAvailabilityTimesModel(CatalogueItem solution, ServiceAvailabilityTimes serviceAvailabilityTimes)
            : this()
        {
            ServiceAvailabilityTimesId = serviceAvailabilityTimes.Id;
            SolutionId = solution.Id;
            SolutionName = solution.Name;
        }

        public int ServiceAvailabilityTimesId { get; init; }

        public CatalogueItemId SolutionId { get; init; }

        public string SolutionName { get; init; }
    }
}
