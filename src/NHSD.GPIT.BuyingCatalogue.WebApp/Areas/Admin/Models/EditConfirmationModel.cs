using System;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class EditConfirmationModel : NavBaseModel
    {
        public EditConfirmationModel(string organisationName, Guid organisationId)
        {
            Name = organisationName;
            BackLink = $"/admin/organisations/{organisationId}";
            BackLinkText = "Back to dashboard";
        }

        public string Name { get; set; }
    }
}
