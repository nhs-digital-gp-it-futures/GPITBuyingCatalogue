using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public enum TimeUnit
    {
        [Display(Name = "month")]
        [Description("per month")]
        [AmountInYear(12)]
        PerMonth = 1,

        [Display(Name = "year")]
        [Description("per year")]
        [AmountInYear(1)]
        PerYear = 2,
    }
}
