using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.Registration
{
    public class RegistrationDetailsModel : NavBaseModel
    {
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(256)]
        public string EmailAddress { get; set; }

        [StringLength(10)]
        public string OdsCode { get; set; }

        [StringLength(1500)]
        public string Justification { get; set; }

        public bool HasReadPrivacyPolicy { get; set; }

        public bool HasGivenUserResearchConsent { get; set; }
    }
}
