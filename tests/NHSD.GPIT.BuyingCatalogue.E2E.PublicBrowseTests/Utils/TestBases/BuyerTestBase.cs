using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases
{
    public abstract class BuyerTestBase : TestBase
    {
        protected BuyerTestBase(LocalWebApplicationFactory factory, Type controller, string methodName, IDictionary<string, string> parameters)
            : base(factory, GenerateUrlFromMethod(controller, methodName, parameters))
        {
            BuyerLogin();
        }

        protected static string GenerateUrlFromMethod(
            Type controller,
            string methodName,
            IDictionary<string, string> parameters,
            IDictionary<string, string> queryParameters = null)
        {
            if (controller.BaseType != typeof(Controller))
                throw new InvalidOperationException($"{nameof(controller)} is not a type of {nameof(Controller)}");

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName), $"{nameof(methodName)} should not be null");

            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters), $"{nameof(parameters)} should not be null");

            var controllerRoute =
                (controller.GetCustomAttributes(typeof(RouteAttribute), false)
                            ?.FirstOrDefault() as RouteAttribute)
                                ?.Template;

            var methodRoute =
                (controller.GetMethods()
                .Where(m =>
                    m.Name == methodName
                    && m.GetCustomAttributes(typeof(HttpGetAttribute), false)
                        .Any())
                        ?.FirstOrDefault()
                        .GetCustomAttributes(typeof(HttpGetAttribute), false)
                            .FirstOrDefault() as HttpGetAttribute)?.Template;

            var absoluteRoute = methodRoute switch
            {
                null => new StringBuilder(controllerRoute.ToLowerInvariant()),
                _ => methodRoute[0] != '~'
                    ? new StringBuilder(controllerRoute.ToLowerInvariant() + "/" + methodRoute.ToLowerInvariant())
                    : new StringBuilder(methodRoute[2..].ToLowerInvariant()),
            };

            if (parameters.Any())
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
                Regex rx = new Regex(@"{[^}]*}", RegexOptions.IgnoreCase);

                var exceptionMessage = $"Not all Parameters in the URL String has been given values." +
                    $" Theses Parameters are missing {string.Join(",", rx.Matches(absoluteRouteUrl).Select(m => m.Value))}";

                throw new InvalidOperationException(exceptionMessage);
            }

            return new Uri("https://www.fake.com/" + absoluteRoute.ToString(), UriKind.Absolute)
                .PathAndQuery[1..];
        }
    }
}
