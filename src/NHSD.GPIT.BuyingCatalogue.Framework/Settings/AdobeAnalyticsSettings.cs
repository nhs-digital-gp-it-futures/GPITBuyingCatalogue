using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    // ReSharper disable once ClassNeverInstantiated.Global (instantiated by framework)
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "init required for deserialization")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "init properties must be public for deserialization")]
    public sealed class AdobeAnalyticsSettings
    {
        public Uri BaseUrl { get; init; }

        public string FileName { get; init; }

        public string ProgrammeArea { get; init; }

        public Uri ScriptSource => new(BaseUrl, FileName);
    }
}
