using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;

namespace NHSD.GPIT.BuyingCatalogue.Services.Session
{
    public sealed class SessionService : ISessionService
    {
        private readonly ISession session;

        public SessionService(IHttpContextAccessor accessor)
        {
            if (accessor is null)
                throw new ArgumentNullException(nameof(accessor));

            session = accessor.HttpContext?.Session
                ?? throw new InvalidOperationException("HttpContext or HttpContext.Session is null");
        }

        public string GetString(string key)
        {
            return session.GetString(key);
        }

        public void SetString(string key, string value)
        {
            session.SetString(key, value);
        }

        public T GetObject<T>(string key)
        {
            var value = session.GetString(key);
            return value is null ? default : JsonDeserializer.Deserialize<T>(value, ReferenceHandler.Preserve);
        }

        public void SetObject(string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve }));
        }

        public void ClearSession(string key)
        {
            session.Remove(key);
        }
    }
}
