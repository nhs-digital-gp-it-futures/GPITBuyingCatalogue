using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

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
