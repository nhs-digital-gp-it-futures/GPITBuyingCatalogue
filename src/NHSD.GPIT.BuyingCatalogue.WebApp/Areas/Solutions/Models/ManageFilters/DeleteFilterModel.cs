using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters
{
    public class DeleteFilterModel : NavBaseModel
    {
        public DeleteFilterModel()
        {
        }

        public DeleteFilterModel(int filterId, string filterName)
        {
            FilterId = filterId;
            FilterName = filterName;
        }

        public string FilterName { get; set; }

        public int FilterId { get; set; }
    }
}
