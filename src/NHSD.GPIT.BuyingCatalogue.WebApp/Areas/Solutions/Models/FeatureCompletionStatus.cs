using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public enum FeatureCompletionStatus
    {
        [Display(Name = "Not started")]
        NotStarted,

        [Display(Name = "In progress")]
        InProgress,

        [Display(Name = "Completed")]
        Completed,
    }
}
