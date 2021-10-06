namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    public sealed class AnalyticsSettings
    {
        public const string Key = "analytics";

        public AdobeAnalyticsSettings Adobe { get; init; }

        public HotjarSettings Hotjar { get; init; }
    }
}
