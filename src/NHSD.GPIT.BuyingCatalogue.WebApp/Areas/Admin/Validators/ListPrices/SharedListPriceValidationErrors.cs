namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    internal static class SharedListPriceValidationErrors
    {
        internal const string MissingTiersError = "Add at least 1 tier";
        internal const string InvalidStartingRangeError = "Lowest tier must have a low range of 1";
        internal const string InvalidEndingRangeError = "Highest tier must have an infinite upper range";
        internal const string RangeOverlapError = "A tier’s lower range overlaps with another tier’s upper range";
        internal const string RangeGapError = "There’s a gap between a tier’s upper range and another tier’s lower range";
    }
}
