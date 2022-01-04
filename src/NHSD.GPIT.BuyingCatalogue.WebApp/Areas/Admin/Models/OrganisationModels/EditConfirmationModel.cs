using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
{
    public sealed class EditConfirmationModel : NavBaseModel
    {
        public EditConfirmationModel(string organisationName, int organisationId)
        {
            Name = organisationName;
            BackLinkText = "Back to dashboard";
        }

        public string Name { get; set; }
    }
}
