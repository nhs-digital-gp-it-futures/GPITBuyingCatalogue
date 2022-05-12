using Microsoft.AspNetCore.WebUtilities;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.Extensions
{
    public static class DriverExtensions
    {
        public static string GetQueryValue(this IWebDriver driver, string queryStringKey)
        {
            var uri = driver.GetUri();
            var queryString = QueryHelpers.ParseQuery(uri.Query);

            if (!queryString.ContainsKey(queryStringKey))
                return string.Empty;

            return queryString[queryStringKey];
        }

        internal static Uri GetUri(this IWebDriver driver)
                    => new(driver.Url);
    }
}
