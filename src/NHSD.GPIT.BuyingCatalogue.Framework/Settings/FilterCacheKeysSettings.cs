using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class FilterCacheKeysSettings
    {
        public const string SectionName = "filterCacheKeys";

        public string FrameworkFilterKey { get; set; }

        public string CancellationSourceKey { get; set; }
    }
}
