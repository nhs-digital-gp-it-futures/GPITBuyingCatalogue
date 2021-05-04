using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class FindOrganisationModel : NavBaseModel
    {
        public FindOrganisationModel()
        {            
            BackLink = "/admin/organisations";
        }

        public string OdsCode { get; set; }
    }
}
