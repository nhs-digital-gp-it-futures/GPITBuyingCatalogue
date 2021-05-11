using System;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class BooleanExtensions
    {
        public static string ToStatus(this bool? value) => value.GetValueOrDefault() ? "COMPLETE" : "INCOMPLETE";

        public static string ToYesNo(this bool? value) => value.GetValueOrDefault() ? "Yes" : "No";
    }
}
