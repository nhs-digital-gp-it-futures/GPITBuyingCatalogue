using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class ContactUsConfirmationModel : NavBaseModel
    {
        private static readonly Dictionary<ContactUsModel.ContactMethodTypes, string> ContactTeams = new()
        {
            { ContactUsModel.ContactMethodTypes.TechnicalFault, "Helpdesk Team" },
            { ContactUsModel.ContactMethodTypes.Other, "Buying Catalogue Team" },
        };

        public ContactUsConfirmationModel()
        {
            BackLinkText = "Go back to homepage";
        }

        public ContactUsConfirmationModel(ContactUsModel.ContactMethodTypes contactReason)
            : this()
        {
            ContactTeam = ContactTeams[contactReason];
        }

        public string ContactTeam { get; set; }
    }
}
