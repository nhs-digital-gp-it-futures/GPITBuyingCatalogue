using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class ImplementationTimescalesModel : NavBaseModel
    {
        public ImplementationTimescalesModel()
        {
        }

        public ImplementationTimescalesModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}";
            BackLinkText = "Return to all sections";

            SolutionId = catalogueItem.CatalogueItemId;
            Description = catalogueItem.Solution.ImplementationDetail;
        }

        public string SolutionId { get; set; }
        public string Description { get; set; }
    }
}
