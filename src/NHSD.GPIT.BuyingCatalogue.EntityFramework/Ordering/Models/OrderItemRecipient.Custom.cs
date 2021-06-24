using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItemRecipient
    {
        public decimal CalculateTotalCostPerYear(decimal price, TimeUnit? timePeriod)
        {
            return price * Quantity * (timePeriod?.AmountInYear() ?? 1);
        }
    }
}
