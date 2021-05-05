using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using System;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class AddUserConfirmationModel : NavBaseModel
    {
        public AddUserConfirmationModel(string userName, Guid organisationId)
        {
            Name = userName;
            BackLink = $"/admin/organisations/{organisationId}";
            BackLinkText = "Back to dashboard";
        }        

        public string Name { get; set; }
    }
}
