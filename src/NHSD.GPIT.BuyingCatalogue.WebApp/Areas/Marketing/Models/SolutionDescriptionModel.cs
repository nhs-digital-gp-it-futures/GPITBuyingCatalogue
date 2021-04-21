using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class SolutionDescriptionModel
    {
        public SolutionDescriptionModel()
        {
        }

        public SolutionDescriptionModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;
            Summary = catalogueItem.Solution.Summary;
            Description = catalogueItem.Solution.FullDescription;
            Link = catalogueItem.Solution.AboutUrl;            
        }
      
        public string SolutionId { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }      
    }
}
