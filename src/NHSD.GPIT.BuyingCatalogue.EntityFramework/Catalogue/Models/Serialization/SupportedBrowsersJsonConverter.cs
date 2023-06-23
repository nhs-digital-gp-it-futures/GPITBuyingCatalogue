using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.Serialization
{
    public sealed class SupportedBrowsersJsonConverter : JsonConverter<HashSet<SupportedBrowser>>
    {
        // TODO: MJK review
        // Used for backward compatability
        // can read both HashSet<SupportedBrowser> and new HashSet<string>()
        // https://github.com/nhs-digital-gp-it-futures/GPITBuyingCatalogue/commit/9d9235e06bd2a7aed2bfce11eba7c5147581b70a
        public override HashSet<SupportedBrowser> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var output = new HashSet<SupportedBrowser>();

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                while (reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        var supportedBrowser = new SupportedBrowser();
                        Type supportedBrowserType = typeof(SupportedBrowser);
                        reader.Read(); // move to first property name token
                        while (reader.TokenType != JsonTokenType.EndObject)
                        {
                            var propertyName = reader.GetString();
                            reader.Read(); // move from propertyName token to value token
                            var propertyValue = reader.GetString();

                            var property = supportedBrowserType.GetProperty(propertyName);
                            property.SetValue(supportedBrowser, propertyValue);

                            reader.Read(); // move to next property in object
                        }

                        output.Add(supportedBrowser);
                    }

                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var supportedBrowser = new SupportedBrowser
                        {
                            BrowserName = reader.GetString(),
                        };
                        output.Add(supportedBrowser);
                    }

                    reader.Read(); // move to next item in array
                }
            }

            return output;
        }

        public override void Write(Utf8JsonWriter writer, HashSet<SupportedBrowser> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
