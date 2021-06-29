using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;

namespace NHSD.GPIT.BuyingCatalogue.Services.Session
{
    public sealed class SessionService : ISessionService
    {
        private readonly ISession session;

        public SessionService(IHttpContextAccessor accessor)
        {
            session = accessor.HttpContext.Session;
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
            return value is null ? default : JsonSerializer.Deserialize<T>(value, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
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
