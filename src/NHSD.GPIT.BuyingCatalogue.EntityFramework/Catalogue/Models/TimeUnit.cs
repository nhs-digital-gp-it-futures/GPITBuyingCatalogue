using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum TimeUnit
    {
        [Display(Name = "month")]
        [Description("per month")]
        PerMonth = 1,

        [Display(Name = "year")]
        [Description("per year")]
        PerYear = 2,
    }
}
