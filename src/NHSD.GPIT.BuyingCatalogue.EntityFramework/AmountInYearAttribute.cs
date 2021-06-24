using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class AmountInYearAttribute : Attribute
    {
        public AmountInYearAttribute(int amountInYear)
        {
            AmountInYear = amountInYear;
        }

        public int AmountInYear { get; }
    }
}
