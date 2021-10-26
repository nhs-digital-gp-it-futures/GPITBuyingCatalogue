using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ImplementationTimescalesModel : SolutionDisplayBaseModel
    {
        public ImplementationTimescalesModel(CatalogueItem item)
            : base(item)
        {
            Description = item.Solution.ImplementationDetail;
        }

        public string Description { get; }

        public override int Index => 8;
    }
}
