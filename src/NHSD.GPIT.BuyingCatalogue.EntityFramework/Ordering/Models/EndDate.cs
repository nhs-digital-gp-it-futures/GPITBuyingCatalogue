using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

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

        public bool ContractExpired => DateTime.HasValue && System.DateTime.UtcNow.Date > DateTime.Value.Date;

        public int RemainingDays(DateTime date)
        {
            if (!DateTime.HasValue)
            {
                throw new InvalidOperationException("A known end date is required to calculate the remaining number of days");
            }

            return Math.Max(0, (DateTime.Value.Date - date.Date).Days);
        }

        public int RemainingTermInMonths(DateTime plannedDelivery)
        {
            if (!DateTime.HasValue)
            {
                throw new InvalidOperationException("A known end date is required to calculate the remaining term");
            }

            return Math.Max(0, DifferenceInMonths(plannedDelivery, DateTime.Value.AddDays(1)));
        }

        public EventTypeEnum DetermineEventToRaise(DateTime date, ICollection<OrderEvent> orderEvents)
        {
            if (!DateTime.HasValue) return EventTypeEnum.Nothing;

            int remainingDays = RemainingDays(date);

            return MaximumTerm >= 3
                ? DetermineEventToRaiseForThresholds(orderEvents, remainingDays, 90, 45)
                : DetermineEventToRaiseForThresholds(orderEvents, remainingDays, 30, 14);
        }

        private static EventTypeEnum DetermineEventToRaiseForThresholds(ICollection<OrderEvent> orderEvents, int remainingDays, int firstThreshold, int secondThreshold)
        {
            if (remainingDays <= 0)
                return EventTypeEnum.Nothing;

            if (remainingDays <= secondThreshold)
            {
                return orderEvents.Any(e => e.EventTypeId == (int)EventTypeEnum.OrderEnteredSecondExpiryThreshold)
                    ? EventTypeEnum.Nothing
                    : EventTypeEnum.OrderEnteredSecondExpiryThreshold;
            }

            if (remainingDays <= firstThreshold)
            {
                return orderEvents.Any(e => e.EventTypeId == (int)EventTypeEnum.OrderEnteredFirstExpiryThreshold)
                    ? EventTypeEnum.Nothing
                    : EventTypeEnum.OrderEnteredFirstExpiryThreshold;
            }

            return EventTypeEnum.Nothing;
        }

        private int DifferenceInMonths(DateTime plannedDelivery, DateTime endDate)
        {
            return ((endDate.Year - plannedDelivery.Year) * 12) + (endDate.Month - plannedDelivery.Month);
        }
    }
}
