using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class FindOrganisationModel : NavBaseModel
    {
        public FindOrganisationModel()
        {
        }

        public FindOrganisationModel(string odsCode)
        {
            OdsCode = odsCode;
            BackLink = "/admin/buyer-organisations";
        }

        public string OdsCode { get; set; }
    }
}
