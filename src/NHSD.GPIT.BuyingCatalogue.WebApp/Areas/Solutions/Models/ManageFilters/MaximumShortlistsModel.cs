using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters
{
    public class MaximumShortlistsModel : NavBaseModel
    {
        public MaximumShortlistsModel(
            string organisationName)
        {
            OrganisationName = organisationName;
        }

        public string OrganisationName { get; set; }
    }
}
