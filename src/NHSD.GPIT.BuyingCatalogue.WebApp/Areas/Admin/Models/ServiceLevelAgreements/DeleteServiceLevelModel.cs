using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements
{
    public class DeleteServiceLevelModel : NavBaseModel
    {
        public DeleteServiceLevelModel()
        {
        }

        public DeleteServiceLevelModel(CatalogueItem solution, SlaServiceLevel serviceLevel)
            : this()
        {
            ServiceLevelId = serviceLevel.Id;
            SolutionId = solution.Id;
            SolutionName = solution.Name;
        }

        public int ServiceLevelId { get; init; }

        public CatalogueItemId SolutionId { get; init; }

        public string SolutionName { get; init; }
    }
}
