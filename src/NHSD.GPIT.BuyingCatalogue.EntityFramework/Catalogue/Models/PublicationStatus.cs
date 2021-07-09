using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum PublicationStatus
    {
        [Display(Name = nameof(Draft), Order = 1)]
        Draft = 1,

        [Display(Name = nameof(Unpublished), Order = 3)]
        Unpublished = 2,

        [Display(Name = nameof(Published), Order = 2)]
        Published = 3,

        [Display(Name = nameof(Suspended), Order = 5)]
        Suspended = 4,

        [Display(Name = "In remediation", Order = 4)]
        InRemediation = 5,
    }
}
