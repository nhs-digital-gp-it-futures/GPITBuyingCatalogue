using System;
using System.Text.Json.Serialization;
using Azure.Core;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    public sealed class AzureBlobStorageRetrySettings
    {
        public TimeSpan Delay { get; set; }

        public TimeSpan MaxDelay { get; set; }

        public int MaxRetries { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RetryMode Mode { get; set; }
    }
}
