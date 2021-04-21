using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
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
