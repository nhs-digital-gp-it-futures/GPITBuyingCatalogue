using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common
{
    internal sealed class CommonActions : ActionBase
    {
        internal CommonActions(IWebDriver driver)
            : base(driver)
        {
        }

        // Click or Select Actions
        internal void ClickGoBackLink() =>
            Driver.FindElement(CommonSelectors.GoBackLink).Click();

        internal void ClickLinkElement(By targetElement) =>
            Driver.FindElement(targetElement).Click();

        internal void ClickSave() =>
            Driver.FindElement(CommonSelectors.SaveAndReturn).Click();

        internal void ClickFirstCheckbox() =>
            Driver.FindElements(By.CssSelector("input[type=checkbox]")).First().Click();

        internal void ClickSection(By targetField, string section) =>
            Driver.FindElements(targetField)
                .Single(s => s.Text.Contains(section)).FindElement(By.TagName("a"))
                .Click();

        internal void ClickRadioButtonWithText(string label) =>
            Driver.FindElements(CommonSelectors.RadioButtonItems)
                .Single(r => r.FindElement(By.TagName("label")).Text == label)
                .FindElement(By.TagName("input"))
                .Click();

        internal string ClickCheckbox(By targetField, int index = 0)
        {
            Driver.FindElements(targetField)[index].FindElement(By.TagName("input")).Click();
            return Driver.FindElements(targetField)[index].FindElement(By.TagName("label")).Text;
        }

        internal void SelectDropdownItem(By targetField, int index = 0) =>
            new SelectElement(Driver.FindElement(targetField)).SelectByIndex(index);

        // Input Element Actions
        internal void ClearInputElement(By targetElement) =>
            Driver.FindElement(targetElement).Clear();

        internal void ElementAddValue(By targetElement, string value)
        {
            Driver.FindElement(targetElement).Clear();
            Driver.FindElement(targetElement).SendKeys(value);
        }

        // Element Displayed
        internal bool ElementIsDisplayed(By targetElement) =>
            ElementExists(targetElement) && Driver.FindElement(targetElement).Displayed;

        internal bool SaveButtonDisplayed() =>
            ElementIsDisplayed(CommonSelectors.SaveAndReturn);

        internal bool GoBackLinkDisplayed() =>
            ElementIsDisplayed(CommonSelectors.GoBackLink);

        internal bool ErrorSummaryDisplayed() =>
            ElementIsDisplayed(CommonSelectors.NhsErrorSection);

        internal bool ElementExists(By targetElement)
        {
            try
            {
                Driver.FindElement(targetElement);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal int GetNumberOfRadioButtonsDisplayed() =>
            Driver.FindElements(By.ClassName("nhsuk-radios__input")).Count;

        // Get Element Values
        internal string PageTitle() =>
            FormatStringForComparison(Driver.FindElement(CommonSelectors.Header1).Text);

        // Element Comparisons

        /// <summary>
        /// Returns if the referenced Element's Error message is equal to the Expected Error Message.
        /// </summary>
        /// <param name="dataValMessage">This will be the value of data-valmgs-for on the Error span when the elements is in error.</param>
        /// <param name="errorMessage">the expected error message.</param>
        /// <returns>true if error message is same as expected, false if not.</returns>
        internal bool ElementShowingCorrectErrorMessage(string dataValMessage, string errorMessage) =>
            ElementShowingCorrectErrorMessage(ByExtensions.DataValMessage(dataValMessage), errorMessage);

        internal bool ElementShowingCorrectErrorMessage(By targetElement, string errorMessage) =>
            FormatStringForComparison(Driver.FindElement(targetElement).Text)
            == FormatStringForComparison(errorMessage);

        internal bool ElementTextEqualToo(By targetElement, string expectedText) =>
            FormatStringForComparison(Driver.FindElement(targetElement).Text)
            .Equals(FormatStringForComparison(expectedText), StringComparison.InvariantCultureIgnoreCase);

        internal bool InputValueEqualToo(By targetElement, string expectedText) =>
            FormatStringForComparison(Driver.FindElement(targetElement).GetAttribute("value"))
            .Equals(FormatStringForComparison(expectedText), StringComparison.InvariantCultureIgnoreCase);

        internal bool InputElementIsEmpty(By targetElement) =>
            string.IsNullOrWhiteSpace(FormatStringForComparison(Driver.FindElement(targetElement).GetAttribute("value")));

        // Element Utils

        /// <summary>
        /// Formats a string by removing all newline and whitespace so that we get more consistent comparisons.
        /// </summary>
        /// <param name="formatString">the string to format.</param>
        /// <returns>a formatted string.</returns>
        internal string FormatStringForComparison(string formatString) =>
            new(formatString.Where(c => !char.IsWhiteSpace(c)).ToArray());

        internal bool PageLoadedCorrectGetIndex(
            Type controllerType,
            string methodName)
        {
            if (controllerType.BaseType != typeof(Controller))
                throw new InvalidOperationException($"{nameof(controllerType)} is not a type of {nameof(Controller)}");

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName), $"{nameof(methodName)} should not be null");

            Wait.Until(d => d.FindElement(CommonSelectors.Header1));

            var controllerRoute =
                (controllerType
                    .GetCustomAttributes(typeof(RouteAttribute), false)
                    ?.FirstOrDefault() as RouteAttribute)?.Template;

            var methodRoute =
                (controllerType
                .GetMethods()
                .Where(m =>
                    m.Name == methodName
                    && m.GetCustomAttributes(typeof(HttpGetAttribute), false)
                    .Any())
                ?.FirstOrDefault()
                .GetCustomAttributes(typeof(HttpGetAttribute), false)
                .FirstOrDefault() as HttpGetAttribute)?.Template;

            var absoluteRoute = methodRoute switch
            {
                null => controllerRoute,
                _ => methodRoute[0] != '~'
                    ? controllerRoute + "/" + methodRoute
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
    }
}
