using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class DeleteListPriceModel : NavBaseModel
    {
        public DeleteListPriceModel()
        {
        }

        public DeleteListPriceModel(
            CatalogueItem solution,
            int cataloguePriceId)
        {
            CataloguePriceId = cataloguePriceId;
            SolutionName = solution.Name;
        }

        public int CataloguePriceId { get; init; }

        public string SolutionName { get; init; }
    }
}
