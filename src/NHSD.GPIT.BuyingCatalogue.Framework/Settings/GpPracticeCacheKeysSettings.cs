using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class GpPracticeCacheKeysSettings
    {
        public const string SectionName = "gpPracticeCacheKeys";

        public string GpPracticeKey { get; set; }

        public string CancellationSourceKey { get; set; }
    }
}
