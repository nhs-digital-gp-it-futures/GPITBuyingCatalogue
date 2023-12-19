using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class AnalyticsSettings
    {
        public const string Key = "analytics";

        public GoogleAnalyticsSettings GoogleAnalytics { get; init; }
    }
}
