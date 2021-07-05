using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public enum PublicationStatus
    {
        [Display(Name = nameof(Draft))]
        Draft = 1,

        [Display(Name = nameof(Unpublished))]
        Unpublished = 2,

        [Display(Name = nameof(Published))]
        Published = 3,

        [Display(Name = nameof(Suspended))]
        Suspended = 4,

        [Display(Name = "In Remediation")]
        InRemediation = 5,
    }
}
