using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class AddAnOrganisationErrorModel : NavBaseModel
    {
        public AddAnOrganisationErrorModel()
        {
        }

        public AddAnOrganisationErrorModel(string odsCode, string error)
        {
            OdsCode = odsCode;
            Error = error;
            BackLinkText = "Back to Find an organisation page";
            BackLink = "/admin/organisations/find";
        }

        public string OdsCode { get; set; }

        public string Error { get; set; }
    }
}
