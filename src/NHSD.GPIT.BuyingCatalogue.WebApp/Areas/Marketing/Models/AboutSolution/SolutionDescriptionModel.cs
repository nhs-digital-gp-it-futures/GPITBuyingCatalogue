using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class SolutionDescriptionModel : NavBaseModel
    {
        public SolutionDescriptionModel()
        {
        }

        public SolutionDescriptionModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}";
            BackLinkText = "Return to all sections";

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
