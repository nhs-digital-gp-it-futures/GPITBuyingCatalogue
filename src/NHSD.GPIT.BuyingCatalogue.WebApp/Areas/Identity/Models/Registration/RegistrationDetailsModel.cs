using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.Registration
{
    public class RegistrationDetailsModel : NavBaseModel
    {
        [StringLength(500)]
        public string FullName { get; set; }

        [StringLength(500)]
        public string EmailAddress { get; set; }

        [StringLength(500)]
        public string OrganisationName { get; set; }

        [StringLength(10)]
        public string OdsCode { get; set; }

        public bool HasReadPrivacyPolicy { get; set; }

        public bool HasGivenUserResearchConsent { get; set; }
    }
}
