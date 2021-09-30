using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Session
{
    public sealed class SessionHandler
    {
        private readonly string sessionKey;
        private readonly IDistributedCache cache;
        private readonly ILoggerFactory loggerFactory;

        public SessionHandler(
            IDataProtectionProvider protectionProvider,
            IDistributedCache cache,
            IWebDriver driver,
            ILoggerFactory loggerFactory)
        {
            sessionKey = GetSessionKeyFromSessionCookie(driver, protectionProvider);
            this.cache = cache;
            this.loggerFactory = loggerFactory;

            RefreshSession();
        }

        private DistributedSession Session { get; set; }

        public string GetString(string key)
        {
            RefreshSession();

            return Session.GetString(key);
        }

        public T GetObject<T>(string key)
        {
            RefreshSession();

            var value = Session.GetString(key);
            return value is null ?
                default :
                JsonSerializer.Deserialize<T>(
                    value,
                    new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve, PropertyNameCaseInsensitive = true });
        }

        public CreateOrderItemModel GetOrderStateFromSession(string key)
        {
            return GetObject<CreateOrderItemModel>(key);
        }

        public Task SetOrderStateToSessionAsync(CreateOrderItemModel model)
        {
            return SetObjectAsync(model.CallOffId.ToString(), model);
        }

        public Task SetStringAsync(string key, string value)
        {
            Session.SetString(key, value);
            return Session.CommitAsync();
        }

        public Task SetObjectAsync(string key, object value)
        {
            Session.SetString(
                key,
                JsonSerializer.Serialize(
                    value,
                    new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve }));

            return Session.CommitAsync();
        }

        public Task Clear()
        {
            Session.Clear();
            return Session.CommitAsync();
        }

        private static string Pad(string text)
        {
            var padding = 3 - ((text.Length + 3) % 4);
            if (padding == 0)
            {
                return text;
            }

            return text + new string('=', padding);
        }

        private static string GetSessionKeyFromSessionCookie(IWebDriver driver, IDataProtectionProvider protectionProvider)
        {
            var protector = protectionProvider.CreateProtector("SessionMiddleware");

            var cookieValue = driver.Manage().Cookies.GetCookieNamed(".AspNetCore.Session").Value;

            var decodedCookieValue = System.Web.HttpUtility.UrlDecode(cookieValue);

            var protectedData = Convert.FromBase64String(Pad(decodedCookieValue));

            var unprotectedData = protector.Unprotect(protectedData);

            return Encoding.UTF8.GetString(unprotectedData);
        }

        private void RefreshSession()
        {
            Session = new DistributedSession(
            cache,
            sessionKey,
            TimeSpan.FromMinutes(20),
            TimeSpan.FromMinutes(1),
            () => true,
            loggerFactory,
            false);
        }
    }
}
