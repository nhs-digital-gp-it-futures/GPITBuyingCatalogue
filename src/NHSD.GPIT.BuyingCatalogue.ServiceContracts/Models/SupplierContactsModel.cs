using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class SupplierContactsModel
    {
        public CatalogueItemId SolutionId { get; set; }

        public MarketingContact[] Contacts { get; set; }

        public MarketingContact ContactFor(int contactId) => Contacts?.FirstOrDefault(x => x.Id == contactId);

        public void SetSolutionId()
        {
            if (Contacts is null || !Contacts.Any())
                return;

            foreach (var contact in Contacts)
            {
                contact.SolutionId = SolutionId;
                contact.LastUpdated = DateTime.UtcNow;
            }
        }

        public IList<MarketingContact> ValidContacts() =>
            Contacts?.Where(c => !c.IsEmpty()).ToList() ?? new List<MarketingContact>();

        public IList<MarketingContact> NewAndValidContacts() =>
            Contacts?.Where(c => c.NewAndValid()).ToList() ?? new List<MarketingContact>();
    }
}
