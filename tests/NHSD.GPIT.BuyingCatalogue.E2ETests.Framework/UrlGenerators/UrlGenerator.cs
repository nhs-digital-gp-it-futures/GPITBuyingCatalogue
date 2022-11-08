using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.UrlGenerators
{
    public static class UrlGenerator
    {
        public static string GenerateUrlFromMethod(
            Type controllerType,
            string methodName,
            IDictionary<string, string> parameters = null,
            IDictionary<string, string> queryParameters = null)
        {
            if (!typeof(Controller).IsAssignableFrom(controllerType))
                throw new InvalidOperationException($"{nameof(controllerType)} is not a type of {nameof(Controller)}");

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName), $"{nameof(methodName)} should not be null");

            var controllerRoute = controllerType.GetCustomAttribute<RouteAttribute>(false)?.Template ?? string.Empty;

            var methodRoute = controllerType.GetMethods()
                .FirstOrDefault(m => m.Name == methodName && m.GetCustomAttribute<HttpGetAttribute>(false) is not null)?
                .GetCustomAttribute<HttpGetAttribute>(false)?.Template;

            var absoluteRoute = methodRoute switch
            {
                null => new StringBuilder(controllerRoute.ToLowerInvariant()),
                _ => methodRoute[0] != '~'
                    ? new StringBuilder(controllerRoute.ToLowerInvariant() + "/" + methodRoute.ToLowerInvariant())
                    : new StringBuilder(methodRoute[2..].ToLowerInvariant()),
            };

            if (parameters is not null && parameters.Any())
            {
                foreach (var param in parameters)
                    absoluteRoute.Replace('{' + param.Key.ToLowerInvariant() + '}', param.Value);
            }

            if (queryParameters is not null && queryParameters.Any())
            {
                absoluteRoute.Append('?');
                foreach (var param in queryParameters)
                    absoluteRoute.Append($"{param.Key}={param.Value}&");

                absoluteRoute.Remove(absoluteRoute.Length - 1, 1);
            }

            var absoluteRouteUrl = absoluteRoute.ToString();

            if (absoluteRouteUrl.Contains('{'))
            {
                Regex rx = new(@"{[^}]*}", RegexOptions.IgnoreCase);

                var exceptionMessage = $"Not all Parameters in the URL String has been given values." +
                    $" Theses Parameters are missing {string.Join(",", rx.Matches(absoluteRouteUrl).Select(m => m.Value))}";

                throw new InvalidOperationException(exceptionMessage);
            }

            return new Uri("https://www.fake.com/" + absoluteRoute.ToString(), UriKind.Absolute)
                .PathAndQuery[1..];
        }
    }
}
