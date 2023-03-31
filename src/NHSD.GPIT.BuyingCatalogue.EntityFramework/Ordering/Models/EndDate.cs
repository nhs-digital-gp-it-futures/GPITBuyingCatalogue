using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public record EndDate(DateTime? CommencementDate, int? MaximumTerm)
    {
        public DateTime? DateTime
        {
            get
            {
                if (CommencementDate == null
                    || MaximumTerm == null)
                    return null;

                return CommencementDate.Value.AddMonths(MaximumTerm.Value).AddDays(-1);
            }
        }

        public string DisplayValue => DateTime.HasValue ? $"{DateTime:d MMMM yyyy}" : string.Empty;

        public int RemainingTerm(DateTime plannedDelivery)
        {
            if (!DateTime.HasValue)
            {
                throw new InvalidOperationException("A known end date is required to calculate the remaining term");
            }

            return Math.Max(0, DifferenceInMonths(plannedDelivery, DateTime.Value.AddDays(1)));
        }

        private int DifferenceInMonths(DateTime plannedDelivery, DateTime endDate)
        {
            return ((endDate.Year - plannedDelivery.Year) * 12) + (endDate.Month - plannedDelivery.Month);
        }
    }
}
