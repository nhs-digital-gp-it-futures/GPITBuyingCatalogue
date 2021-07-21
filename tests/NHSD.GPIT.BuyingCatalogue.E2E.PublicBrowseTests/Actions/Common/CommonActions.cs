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

        internal int GetNumberOfRadioButtonsDisplayed =>
                    Driver.FindElements(By.ClassName("nhsuk-radios__input")).Count;

        // Click Actions
        internal void ClickGoBackLink() =>
            Driver.FindElement(CommonSelectors.GoBackLink).Click();

        internal bool GoBackLinkDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElement(CommonSelectors.GoBackLink));
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void ClickSave()
        {
            Driver.FindElement(CommonSelectors.SaveAndReturn).Click();
        }

        internal bool SaveButtonDisplayed()
        {
            return ElementIsDisplayed(CommonSelectors.SaveAndReturn);
        }

        internal void ClickFirstCheckbox() =>
            Driver.FindElements(By.CssSelector("input[type=checkbox]")).First().Click();

        internal string ClickCheckbox(By targetField, int index = 0)
        {
            var checkboxItems = Driver.FindElements(targetField);

            var selected = checkboxItems[index];

            selected.FindElement(By.TagName("input")).Click();
            return selected.FindElement(By.TagName("label")).Text;
        }

        internal void ClickSection(By targetField, string section)
        {
            Driver.FindElements(targetField)
                .Single(s => s.Text.Contains(section)).FindElement(By.TagName("a"))
                .Click();
        }

        internal void SelectDropdownItem(By targetField, int index = 0)
        {
            var select = Driver.FindElement(targetField);
            new SelectElement(select).SelectByIndex(index);
        }

        internal void ClickRadioButtonWithText(string label)
        {
            var radioButtonItems = Driver.FindElements(CommonSelectors.RadioButtonItems);

            radioButtonItems.Single(r => r.FindElement(By.TagName("label")).Text == label).FindElement(By.TagName("input")).Click();
        }

        // testing
        internal bool ErrorSummaryDisplayed()
        {
            try
            {
                Driver.FindElement(CommonSelectors.NhsErrorSection);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Formats a string by removing all newline and whitespace so that we get more consistant comparisons.
        /// </summary>
        /// <param name="formatString">the string to format.</param>
        /// <returns>a formatted string.</returns>
        internal string FormatStringForComparison(string formatString) =>
            formatString.Replace("\r\n", " ").Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();

        internal bool PageLoadedCorrectGetIndex(
            Type controller,
            string methodName)
        {
            if (controller.BaseType != typeof(Controller))
                throw new InvalidOperationException($"{nameof(controller)} is not a type of {nameof(Controller)}");

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName), $"{nameof(methodName)} should not be null");

            Wait.Until(d => d.FindElement(CommonSelectors.Header1));

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
                null => controllerRoute,
                _ => methodRoute[0] != '~'
                    ? controllerRoute + "/" + methodRoute
                    : methodRoute[2..],
            };

            var driverUrl = new Uri(Driver.Url);

            var actionUrl = new Uri("https://www.fake.com/" + absoluteRoute, UriKind.Absolute);

            if (driverUrl.Segments.Length != actionUrl.Segments.Length)
                return false;

            for (int i = 0; i < actionUrl.Segments.Length; i++)
            {
                if (!actionUrl.Segments[i].StartsWith("%7B"))
                {
                    if (driverUrl.Segments[i] != actionUrl.Segments[i])
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns if the referenced Element's Error message is equal to the Expected Error Message.
        /// </summary>
        /// <param name="dataValMessage">This will be the value of data-valmgs-for on the Error span when the elements is in error.</param>
        /// <param name="errorMessage">the expected error message.</param>
        /// <returns>true if error message is same as expected, false if not.</returns>
        internal bool ElementShowingCorrectErrorMessage(string dataValMessage, string errorMessage) =>
            ElementShowingCorrectErrorMessage(ByExtensions.DataValMessage(dataValMessage), errorMessage);

        /// <summary>
        /// Returns if the referenced Element's Error message is equal to the Expected Error Message.
        /// </summary>
        /// <param name="targetElement">the element containing the error message.</param>
        /// <param name="errorMessage">the expected error message.</param>
        /// <returns>true if error message is same as expected, false if not.</returns>
        internal bool ElementShowingCorrectErrorMessage(By targetElement, string errorMessage)
        {
            var errorSpanText = Driver.FindElement(targetElement).Text;

            return FormatStringForComparison(errorSpanText) == FormatStringForComparison(errorMessage);
        }

        internal bool ElementIsDisplayed(By targetElement) =>
            ElementExists(targetElement) && Driver.FindElement(targetElement).Displayed;

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

        internal bool ElementTextEqualToo(By targetElement, string expectedText) =>
            FormatStringForComparison(Driver.FindElement(targetElement).Text)
            .Equals(FormatStringForComparison(expectedText), StringComparison.InvariantCultureIgnoreCase);

        internal bool InputValueEqualToo(By targetElement, string expectedText) =>
            FormatStringForComparison(Driver.FindElement(targetElement).GetAttribute("value"))
            .Equals(FormatStringForComparison(expectedText), StringComparison.InvariantCultureIgnoreCase);

        internal bool InputElementIsEmpty(By targetElement) =>
            string.IsNullOrWhiteSpace(FormatStringForComparison(Driver.FindElement(targetElement).GetAttribute("value")));

        internal void ClearInputElement(By targetElement) =>
            Driver.FindElement(targetElement).Clear();

        internal void ClickLinkElement(By targetElement) =>
            Driver.FindElement(targetElement).Click();

        internal void ElementAddValue(By targetElement, string value)
        {
            Driver.FindElement(targetElement).Clear();
            Driver.FindElement(targetElement).SendKeys(value);
        }

        internal string PageTitle() =>
            FormatStringForComparison(Driver.FindElement(CommonSelectors.Header1).Text);
    }
}
