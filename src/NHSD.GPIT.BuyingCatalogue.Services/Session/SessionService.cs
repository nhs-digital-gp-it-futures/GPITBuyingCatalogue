using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;

namespace NHSD.GPIT.BuyingCatalogue.Services.Session
{
    public class SessionService : ISessionService
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
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public void SetObject(string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public void ClearSession()
        {
            session.Clear();
        }
    }
}
