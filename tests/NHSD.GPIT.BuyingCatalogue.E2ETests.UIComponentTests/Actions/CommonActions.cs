using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Actions
{
    internal sealed class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver) : base(driver)
        {
        }

        internal void ClickElement(By by) => Driver.FindElement(by).Click();

        internal string GetElementChecked(By by) => Driver.FindElement(by).GetDomProperty("checked");

        internal string GetElementText(By by) => Driver.FindElement(by).Text;

        internal void SendTextToElement(By by, string text) => Driver.FindElement(by).SendKeys(text);

        internal string GetElementValue(By by) => Driver.FindElement(by).GetDomProperty("value");

        internal bool IsElementDisplayed(By by) => Driver.FindElement(by).Displayed;

        internal IEnumerable<IWebElement> GetElements(By by) => Driver.FindElements(by);

        internal string GetSelectDropDownValue(By by)
        {
            var selectElement = new SelectElement(Driver.FindElement(by));
            return selectElement.SelectedOption.GetDomAttribute("value");
        }

        internal void SelectDropDownItemByText(By targetField, string text)
        {
            var selectElement = new SelectElement(Driver.FindElement(targetField));
            selectElement.SelectByText(text);
        }

        // Get Element Values
        internal string PageTitle() =>
            Driver.FindElement(CommonSelectors.Header1).Text.FormatForComparison();

        internal bool PageLoadedCorrectGetIndex(
            Type controllerType,
            string methodName)
        {
            if (controllerType.BaseType != typeof(Controller))
                throw new InvalidOperationException($"{nameof(controllerType)} is not a type of {nameof(Controller)}");

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName), $"{nameof(methodName)} should not be null");

            WaitUntilElementExists(CommonSelectors.Header1);

            var controllerRoute = controllerType.GetCustomAttribute<RouteAttribute>(false)?.Template;

            var methodRoute = controllerType.GetMethods()
                .FirstOrDefault(m => m.Name == methodName && m.GetCustomAttribute<HttpGetAttribute>(false) is not null)?
                .GetCustomAttribute<HttpGetAttribute>(false)?.Template;

            var absoluteRoute = methodRoute switch
            {
                null => controllerRoute,
                _ => methodRoute[0] != '~'
                    ? string.Join('/', new[] { controllerRoute, methodRoute }.Where(s => !string.IsNullOrWhiteSpace(s)))
                    : methodRoute[2..],
            };

            var driverUrl = new Uri(Driver.Url);

            var actionUrl = new Uri("https://www.fake.com/" + absoluteRoute, UriKind.Absolute);

            if (driverUrl.Segments.Length != actionUrl.Segments.Length)
                return false;

            // checks every segment in actionUrl, that doesn't start with a "{" (%7B) against the same positioned element in driverUrl.
            // if any don't match, will return false, else true.
            return !actionUrl.Segments
                .Where((t, i) => !t.StartsWith("%7B") && driverUrl.Segments[i] != t)
                .Any();
        }

        internal void WaitUntilElementExists(By element) => Wait.Until(d => d.FindElement(element));
    }
}
