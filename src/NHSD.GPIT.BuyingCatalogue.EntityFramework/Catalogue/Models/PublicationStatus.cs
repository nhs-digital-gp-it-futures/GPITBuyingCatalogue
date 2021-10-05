using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum PublicationStatus
    {
        [Description("Save as draft")]
        [Display(Name = nameof(Draft), Order = 1)]
        Draft = 1,

        [Description("Unpublish")]
        [Display(Name = nameof(Unpublished), Order = 3)]
        Unpublished = 2,

        [Description("Publish")]
        [Display(Name = nameof(Published), Order = 2)]
        Published = 3,

        [Description("Suspend")]
        [Display(Name = nameof(Suspended), Order = 5)]
        Suspended = 4,

        [Description("In remediation")]
        [Display(Name = "In remediation", Order = 4)]
        InRemediation = 5,
    }
}
