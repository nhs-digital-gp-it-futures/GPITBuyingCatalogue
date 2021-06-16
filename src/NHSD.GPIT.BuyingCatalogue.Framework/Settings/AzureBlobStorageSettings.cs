using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Blobs;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    public sealed class AzureBlobStorageSettings
    {
        // Ignored to prevent access keys being logged
        [JsonIgnore]
        public string ConnectionString { get; set; }

        public string ContainerName { get; set; }

        public string DocumentDirectory { get; set; }

        public AzureBlobStorageRetrySettings Retry { get; set; }

        // Not part of the interface definition as its current use
        // is for logging only
        public Uri Uri
        {
            get
            {
                if (ConnectionString == null)
                    return null;

                try
                {
                    return new BlobServiceClient(ConnectionString).Uri;
                }
                catch (FormatException)
                {
                    return null;
                }
            }
        }

        public override string ToString()
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            jsonOptions.Converters.Add(new TimeSpanConverter());

            return JsonSerializer.Serialize(this, jsonOptions);
        }

        private sealed class TimeSpanConverter : JsonConverter<TimeSpan>
        {
            public override TimeSpan Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                throw new NotSupportedException();
            }

            public override void Write(
                Utf8JsonWriter writer,
                TimeSpan value,
                JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
