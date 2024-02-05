using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.OrderType
{
    public enum AssociatedServiceType
    {
        [Display(Name = "Merger")]
        AssociatedServiceMerger = 1,

        [Display(Name = "Split")]
        AssociatedServiceSplit = 2,

        [Display(Name = "Something else")]
        AssociatedServiceOther = 3,
    }
}
