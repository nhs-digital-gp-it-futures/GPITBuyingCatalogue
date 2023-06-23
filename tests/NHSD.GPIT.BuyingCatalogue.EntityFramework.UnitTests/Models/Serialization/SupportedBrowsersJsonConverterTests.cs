using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.Serialization;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Serialization
{
    public sealed class SupportedBrowsersJsonConverterTests
    {
        private readonly HashSet<string> jsonStringArray = new() { "Google Chrome", "Microsoft Edge", "Mozilla Firefox" };

        private readonly HashSet<SupportedBrowser> jsonObjectArray = new()
        {
            new SupportedBrowser { BrowserName = "Google Chrome", MinimumBrowserVersion = "96.0.4664.110" },
            new SupportedBrowser { BrowserName = "Microsoft Edge" },
            new SupportedBrowser { BrowserName = "Mozilla Firefox" },
        };

        [Fact]
        public static void SupportedBrowsersJsonConverter_Read_EmptyHashSetArray_ReturnsNewHashSet()
        {
            var expected = new HashSet<SupportedBrowser>();

            var reader = new Utf8JsonReader(new System.Buffers.ReadOnlySequence<byte>(JsonSerializer.SerializeToUtf8Bytes(expected)));

            var jsonConverter = new SupportedBrowsersJsonConverter();

            reader.Read(); // needed to force it to look at the first token

            var result = jsonConverter.Read(ref reader, expected.GetType(), new JsonSerializerOptions());

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void SupportedBrowsersJsonConverter_Read_EmptystringArray_ReturnsNewHashSet()
        {
            var expected = new HashSet<SupportedBrowser>();

            var input = new HashSet<string>();

            var reader = new Utf8JsonReader(new System.Buffers.ReadOnlySequence<byte>(JsonSerializer.SerializeToUtf8Bytes(input)));

            var jsonConverter = new SupportedBrowsersJsonConverter();

            reader.Read(); // needed to force it to look at the first token

            var result = jsonConverter.Read(ref reader, expected.GetType(), new JsonSerializerOptions());

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void SupportedBrowsersJsonConverter_Read_StringArrayInput_ReturnsHashSet()
        {
            var expected = new HashSet<SupportedBrowser>
            {
                new SupportedBrowser { BrowserName = "Google Chrome" },
                new SupportedBrowser { BrowserName = "Microsoft Edge" },
                new SupportedBrowser { BrowserName = "Mozilla Firefox" },
            };

            var reader = new Utf8JsonReader(new System.Buffers.ReadOnlySequence<byte>(JsonSerializer.SerializeToUtf8Bytes(jsonStringArray)));

            var jsonConverter = new SupportedBrowsersJsonConverter();

            reader.Read(); // needed to force it to look at the first token

            var result = jsonConverter.Read(ref reader, expected.GetType(), new JsonSerializerOptions());

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void SupportedBrowsersJsonConverter_Read_ObjectArrayInput_ReturnsHashSet()
        {
            var expected = new HashSet<SupportedBrowser>
            {
                new SupportedBrowser { BrowserName = "Google Chrome", MinimumBrowserVersion = "96.0.4664.110" },
                new SupportedBrowser { BrowserName = "Microsoft Edge" },
                new SupportedBrowser { BrowserName = "Mozilla Firefox" },
            };

            var reader = new Utf8JsonReader(new System.Buffers.ReadOnlySequence<byte>(JsonSerializer.SerializeToUtf8Bytes(jsonObjectArray)));

            var jsonConverter = new SupportedBrowsersJsonConverter();

            reader.Read(); // needed to force it to look at the first token

            var result = jsonConverter.Read(ref reader, expected.GetType(), new JsonSerializerOptions());

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void SupportedBrowsersJsonConverter_Write_ConvertsToObjectJson()
        {
            var expected = JsonSerializer.Serialize(jsonObjectArray);

            var stream = new System.IO.MemoryStream();

            var writer = new Utf8JsonWriter(stream);

            var jsonConverter = new SupportedBrowsersJsonConverter();

            jsonConverter.Write(writer, jsonObjectArray, new JsonSerializerOptions());

            var output = Encoding.UTF8.GetString(stream.ToArray());

            stream.Dispose();

            output.Should().BeEquivalentTo(expected);
        }
    }
}
