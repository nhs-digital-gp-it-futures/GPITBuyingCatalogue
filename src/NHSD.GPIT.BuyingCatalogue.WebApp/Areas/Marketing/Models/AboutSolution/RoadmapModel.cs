using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class RoadmapModel : NavBaseModel
    {
        public RoadmapModel()
        {
        }

        public RoadmapModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}";
            BackLinkText = "Return to all sections";

            SolutionId = catalogueItem.CatalogueItemId;
            Summary = catalogueItem.Solution.RoadMap;
        }

        public string SolutionId { get; set; }
        public string Summary { get; set; }
    }
}
