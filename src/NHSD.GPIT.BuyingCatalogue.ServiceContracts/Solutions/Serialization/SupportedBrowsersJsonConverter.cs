﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Serialization
{
    public sealed class SupportedBrowsersJsonConverter : JsonConverter<HashSet<SupportedBrowser>>
    {
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
