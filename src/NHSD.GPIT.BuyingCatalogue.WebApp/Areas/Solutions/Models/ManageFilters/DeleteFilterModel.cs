using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters
{
    public class DeleteFilterModel : NavBaseModel
    {
        public DeleteFilterModel()
        {
        }

        public string FilterName { get; set; }

        public int FilterId { get; set; }
    }
}
