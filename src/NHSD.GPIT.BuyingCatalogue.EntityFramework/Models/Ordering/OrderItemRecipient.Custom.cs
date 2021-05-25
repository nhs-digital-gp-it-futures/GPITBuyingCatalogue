namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class OrderItemRecipient
    {
        public decimal CalculateTotalCostPerYear(decimal price, TimeUnit timePeriod)
        {
            return price * Quantity * (timePeriod?.AmountInYear() ?? 1);
        }
    }
}
