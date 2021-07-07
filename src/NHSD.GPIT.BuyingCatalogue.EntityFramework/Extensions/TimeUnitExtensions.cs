using System;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class TimeUnitExtensions
    {
        public static string Description(this TimeUnit timeUnit) => timeUnit.AsString(EnumFormat.Description);

        public static string Name(this TimeUnit timeUnit) => timeUnit.AsString(EnumFormat.DisplayName);

        public static string EnumMemberName(this TimeUnit timeUnit) => timeUnit.ToString();

        internal static int AmountInYear(this TimeUnit timeUnit)
        {
            var amountInYearAttribute = timeUnit.GetAttributes()?.Get<AmountInYearAttribute>();
            if (amountInYearAttribute is null)
                throw new InvalidOperationException();

            return amountInYearAttribute.AmountInYear;
        }
    }
}
