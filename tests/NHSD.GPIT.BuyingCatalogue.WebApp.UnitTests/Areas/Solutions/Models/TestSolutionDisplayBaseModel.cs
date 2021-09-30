using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public class TestSolutionDisplayBaseModel : SolutionDisplayBaseModel
    {
        public TestSolutionDisplayBaseModel()
        {
        }

        public TestSolutionDisplayBaseModel(CatalogueItem item)
            : base(item)
        {
        }

        public override int Index => 8;
    }
}
