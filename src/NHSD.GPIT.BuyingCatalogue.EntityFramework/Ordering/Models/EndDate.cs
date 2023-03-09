using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public record EndDate(DateTime? CommencementDate, int? MaximumTerm, OrderTriageValue? TriageValue = null)
    {
        public const int MaximumTermForOnOffCatalogueOrders = 48;

        public DateTime? Value
        {
            get
            {
                if (CommencementDate == null
                    || MaximumTerm == null)
                    return null;

                return TriageValue switch
                {
                    // direct Award
                    null or OrderTriageValue.Under40K => CommencementDate?.AddMonths(MaximumTerm.Value).AddDays(-1),

                    // on/off
                    _ => (DateTime?)CommencementDate.Value.AddMonths(MaximumTermForOnOffCatalogueOrders).AddDays(-1),
                };
            }
        }

        public string DisplayValue => Value.HasValue ? $"{Value:d MMMM yyyy}" : string.Empty;

        public decimal RemainingTerm(DateTime plannedDelivery)
        {
            if (!Value.HasValue)
            {
                throw new InvalidOperationException("A known end date is required to calculate the remaining term");
            }

            var remainingMonths = Math.Max(0, RemainingMonths(plannedDelivery));

            return TriageValue switch
            {
                // direct Award
                null or OrderTriageValue.Under40K => remainingMonths,

                // on/off
                _ => Math.Min(remainingMonths, MaximumTerm.Value),
            };
        }

        private decimal RemainingMonths(DateTime plannedDelivery)
        {
            if (!Value.HasValue)
                return 0;

            var endDate = Value?.AddDays(1);

            return ((endDate?.Year - plannedDelivery.Year) * 12) + endDate?.Month - plannedDelivery.Month ?? 0;
        }
    }
}
