using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class ValueConverterExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
        {
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var converter = new ValueConverter<T, string>(
                value => JsonSerializer.Serialize(value, null),
                serializedValue => JsonSerializer.Deserialize<T>(serializedValue, jsonSerializerOptions));

            var comparer = new ValueComparer<T>(
                (left, right) => JsonSerializer.Serialize(left, null) == JsonSerializer.Serialize(right, null),
                value => value == null ? 0 : JsonSerializer.Serialize(value, null).GetHashCode(),
                value => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, null), jsonSerializerOptions));

            propertyBuilder.HasConversion(converter, comparer);

            return propertyBuilder;
        }
    }
}
