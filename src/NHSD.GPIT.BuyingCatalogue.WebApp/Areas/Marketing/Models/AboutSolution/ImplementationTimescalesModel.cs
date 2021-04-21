using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution
{
    public class ImplementationTimescalesModel
    {
        public ImplementationTimescalesModel()
        {
        }

        public ImplementationTimescalesModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;
            Description = catalogueItem.Solution.ImplementationDetail;
        }

        public string SolutionId { get; set; }
        public string Description { get; set; }
    }
}
