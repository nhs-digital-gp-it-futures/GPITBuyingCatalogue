using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation
{
    public class ContactDetailsModel : NavBaseModel
    { 
        public ContactDetailsModel()
        {
            Contact1 = new MarketingContact();
            Contact2 = new MarketingContact();
        }

        public ContactDetailsModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}";
            BackLinkText = "Return to all sections";

            SolutionId = catalogueItem.CatalogueItemId;

            var allContacts = catalogueItem.Solution.MarketingContacts.ToArray();

            if (allContacts.Length > 0)
                Contact1 = allContacts[0];
            else
                Contact1 = new MarketingContact();

            if (allContacts.Length > 1)
                Contact2 = allContacts[1];
            else
                Contact2 = new MarketingContact();
        }

        public string SolutionId { get; set; }

        public MarketingContact Contact1 { get; set; }

        public MarketingContact Contact2 { get; set; }
    }
}
