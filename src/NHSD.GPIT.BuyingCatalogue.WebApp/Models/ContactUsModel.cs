using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class ContactUsModel : NavBaseModel
    {
        public enum ContactMethodTypes
        {
            Website = 1,
            Other,
        }

        public IEnumerable<object> ContactMethodOptions =>
            new List<object>
            {
                new { Display = "A technical fault with this website", Value = ContactMethodTypes.Website},
                new { Display = "Any other query about the Buying Catalogue", Value = ContactMethodTypes.Other },
            };

        public string ContactMethod { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string Message { get; set; }

        public bool PrivacyPolicyAccepted { get; set; }
    }
}
