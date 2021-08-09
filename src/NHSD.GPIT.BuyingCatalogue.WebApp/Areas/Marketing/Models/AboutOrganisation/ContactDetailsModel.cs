using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation
{
    public class ContactDetailsModel : MarketingBaseModel
    {
        public ContactDetailsModel()
            : base(null)
        {
            Contact1 = new MarketingContact();
            Contact2 = new MarketingContact();
            ClientApplication = new ClientApplication();
        }

        public ContactDetailsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.Id}";

            var allContacts = CatalogueItem.Solution.MarketingContacts.ToArray();

            Contact1 = allContacts.Length > 0 ? allContacts[0] : new MarketingContact();

            Contact2 = allContacts.Length > 1 ? allContacts[1] : new MarketingContact();
        }

        public override bool? IsComplete => (Contact1 != null && !Contact1.IsEmpty())
                                            || (Contact2 != null && !Contact2.IsEmpty());

        public MarketingContact Contact1 { get; set; }

        public MarketingContact Contact2 { get; set; }
    }
}
