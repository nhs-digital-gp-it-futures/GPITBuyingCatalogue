using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ImplementationTimescalesModel : SolutionDisplayBaseModel
    {
        public ImplementationTimescalesModel(CatalogueItem item, CatalogueItemContentStatus contentStatus)
            : base(item, contentStatus)
        {
            Description = item.Solution.ImplementationDetail;
        }

        public string Description { get; }

        public override int Index => 6;
    }
}
