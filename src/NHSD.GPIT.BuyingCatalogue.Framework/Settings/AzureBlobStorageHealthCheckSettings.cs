using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class AzureBlobStorageHealthCheckSettings
    {
        public TimeSpan Timeout { get; set; }
    }
}
