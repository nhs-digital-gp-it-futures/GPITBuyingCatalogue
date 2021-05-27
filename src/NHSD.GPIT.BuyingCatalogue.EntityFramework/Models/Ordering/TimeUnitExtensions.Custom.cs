using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public static class TimeUnitExtensions
    {
        public static string Description(this TimeUnit timeUnit) => timeUnit.GetAttributeFromEnumProperty<DescriptionAttribute>().Description;

        public static string Name(this TimeUnit timeUnit) => timeUnit.GetAttributeFromEnumProperty<DisplayAttribute>().Name;

        public static int AmountInYear(this TimeUnit timeUnit) => timeUnit.GetAttributeFromEnumProperty<AmountInYearAttribute>().AmountInYear;
    }
}
