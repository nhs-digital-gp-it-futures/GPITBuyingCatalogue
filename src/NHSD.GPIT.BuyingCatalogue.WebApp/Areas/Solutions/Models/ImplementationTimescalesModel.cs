using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ImplementationTimescalesModel : SolutionDisplayBaseModel
    {
        public ImplementationTimescalesModel(CatalogueItem solution)
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;
            Description = solution.Solution.ImplementationDetail;
        }

        public string Description { get; }

        public override int Index => 7;
    }
}
