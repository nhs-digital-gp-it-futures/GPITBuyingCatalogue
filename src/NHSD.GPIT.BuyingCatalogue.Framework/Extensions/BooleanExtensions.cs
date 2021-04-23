namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class BooleanExtensions
    {
        public static string ToYesNo(this bool? value)
        {
            return value.GetValueOrDefault() ? "Yes" : "No";
        }
    }
}
