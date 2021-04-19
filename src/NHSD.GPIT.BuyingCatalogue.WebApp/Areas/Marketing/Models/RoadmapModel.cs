using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class RoadmapModel
    {
        public RoadmapModel()
        {
        }

        public RoadmapModel(CatalogueItem catalogueItem)
        {
            Summary = catalogueItem.Solution.RoadMap;
        }

        public string Id { get; set; }
        public string Summary { get; set; }
    }
}
