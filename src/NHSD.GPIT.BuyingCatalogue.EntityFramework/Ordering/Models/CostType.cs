using System.ComponentModel;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public enum CostType
    {
        [Description("Recurring")]
        Recurring = 0,
        [Description("One-off")]
        OneOff = 1,
    }
}
