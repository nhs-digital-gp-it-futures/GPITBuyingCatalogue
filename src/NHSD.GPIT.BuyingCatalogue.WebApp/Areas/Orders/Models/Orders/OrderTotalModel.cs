using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public class OrderTotalModel
    {
        public OrderTotalModel(
            OrderType orderType,
            int? maximumTerm,
            decimal totalOneOffCost,
            decimal totalMonthlyCost,
            decimal totalAnnualCost,
            decimal totalCost)
        {
            OrderType = orderType;
            MaximumTerm = maximumTerm;
            TotalOneOffCost = totalOneOffCost;
            TotalMonthlyCost = totalMonthlyCost;
            TotalAnnualCost = totalAnnualCost;
            TotalCost = totalCost;
            OneOffCostOnly = totalOneOffCost > 0M
                && totalMonthlyCost == 0M
                && totalAnnualCost == 0M;
        }

        public OrderType OrderType { get; }

        public int? MaximumTerm { get; }

        public decimal TotalOneOffCost { get; }

        public decimal TotalMonthlyCost { get; }

        public decimal TotalAnnualCost { get; }

        public decimal TotalCost { get; }

        public bool OneOffCostOnly { get; }
    }
}
