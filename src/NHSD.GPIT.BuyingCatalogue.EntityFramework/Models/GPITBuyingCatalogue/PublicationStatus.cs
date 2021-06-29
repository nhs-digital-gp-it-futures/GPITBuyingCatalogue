using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public enum PublicationStatus
    {
        [Display(Name = "Suspended")]
        Draft = 1,

        Unpublished = 2,

        Published = 3,

        [Display(Name = "Deleted")]
        Withdrawn = 4,
    }
}
