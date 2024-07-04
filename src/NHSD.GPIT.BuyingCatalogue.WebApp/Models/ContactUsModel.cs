using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class ContactUsModel : NavBaseModel
    {
        [StringLength(1500)]
        public string Message { get; set; }

        [StringLength(500)]
        public string FullName { get; set; }

        [StringLength(500)]
        public string EmailAddress { get; set; }

        public bool PrivacyPolicyAccepted { get; set; }
    }
}
