using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AmountInYearAttribute : Attribute
    {
        public AmountInYearAttribute(int amountInYear) => AmountInYear = amountInYear;

        public int AmountInYear { get; }
    }
}
