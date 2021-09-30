using System.Text.Json;
using System.Text.Json.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Serialization
{
    public static class JsonDeserializer
    {
        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static T Deserialize<T>(string json, ReferenceHandler referenceHandler)
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { ReferenceHandler = referenceHandler, PropertyNameCaseInsensitive = true });
        }
    }
}
