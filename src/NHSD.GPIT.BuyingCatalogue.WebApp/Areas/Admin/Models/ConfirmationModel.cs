using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class ConfirmationModel : NavBaseModel
    {
        public ConfirmationModel(string organisationName)
        {
            Name = organisationName;
            BackLink = "/admin/organisations";
            BackLinkText = "Back to dashboard";
        }

        public string Name { get; set; }
    }
}
