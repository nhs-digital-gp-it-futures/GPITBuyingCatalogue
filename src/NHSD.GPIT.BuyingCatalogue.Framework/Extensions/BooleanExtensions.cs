namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class BooleanExtensions
    {
        public static string ToStatus(this bool? value) => value.GetValueOrDefault() ? "COMPLETE" : "INCOMPLETE";

        public static string ToYesNo(this bool? value)
        {
            if (value == null) return string.Empty;
            return value.Value ? "Yes" : "No";
        }
    }
}
