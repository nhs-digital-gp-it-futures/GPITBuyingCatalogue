using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class ContactUsModel : NavBaseModel
    {
        public enum ContactMethodTypes
        {
            TechnicalFault,
            Other,
        }

        public IEnumerable<object> ContactMethodOptions =>
            new List<object>
            {
                new { Display = "A technical fault with this website", Value = ContactMethodTypes.TechnicalFault },
                new { Display = "Any other query about the Buying Catalogue", Value = ContactMethodTypes.Other },
            };

        public ContactMethodTypes? ContactMethod { get; set; }

        [StringLength(1500)]
        public string Message { get; set; }

        [StringLength(500)]
        public string FullName { get; set; }

        [StringLength(500)]
        public string EmailAddress { get; set; }

        public bool PrivacyPolicyAccepted { get; set; }
    }
}
