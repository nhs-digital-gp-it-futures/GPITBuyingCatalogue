using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class RoadmapModel
    {
        public RoadmapModel()
        {
        }

        public RoadmapModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;
            Summary = catalogueItem.Solution.RoadMap;
        }

        public string SolutionId { get; set; }
        public string Summary { get; set; }
    }
}
