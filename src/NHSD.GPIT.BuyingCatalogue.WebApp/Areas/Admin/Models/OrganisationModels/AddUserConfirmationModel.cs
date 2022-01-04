using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
{
    public sealed class AddUserConfirmationModel : NavBaseModel
    {
        public AddUserConfirmationModel(string userName, int organisationId)
        {
            Name = userName;
            BackLinkText = "Back to dashboard";
        }

        public string Name { get; set; }
    }
}
