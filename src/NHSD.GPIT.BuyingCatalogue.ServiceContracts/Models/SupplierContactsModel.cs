using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public class SupplierContactsModel
    {
        public virtual string SolutionId { get; set; }

        public MarketingContact[] Contacts { get; set; }

        public virtual MarketingContact ContactFor(int contactId) => Contacts?.FirstOrDefault(x => x.Id == contactId);

        public virtual void SetSolutionId()
        {
            if (Contacts == null || !Contacts.Any())
                return;

            foreach (var contact in Contacts)
            {
                contact.SolutionId = SolutionId;
                contact.LastUpdated = DateTime.UtcNow;
            }
        }

        public virtual IList<MarketingContact> ValidContacts() =>
            Contacts?.Where(x => !x.IsEmpty()).ToList() ?? new List<MarketingContact>();

        public virtual IList<MarketingContact> NewAndValidContacts() =>
            Contacts?.Where(x => x.NewAndValid()).ToList() ?? new List<MarketingContact>();
    }
}
