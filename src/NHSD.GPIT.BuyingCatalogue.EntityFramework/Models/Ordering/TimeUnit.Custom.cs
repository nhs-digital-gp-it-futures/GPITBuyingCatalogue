using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class TimeUnit
    {
        public enum TimeUnitType
        {
            PerMonth,
            PerYear,
        }

        public static TimeUnit GetTimeUnitByType(TimeUnitType type) => type switch
        {
            TimeUnitType.PerMonth => new TimeUnit() { Id = 1 },
            TimeUnitType.PerYear => new TimeUnit() { Id = 2 },
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        public int AmountInYear() => Id switch
        {
            1 => 12,
            2 => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(Id)),
        };
    }
}
