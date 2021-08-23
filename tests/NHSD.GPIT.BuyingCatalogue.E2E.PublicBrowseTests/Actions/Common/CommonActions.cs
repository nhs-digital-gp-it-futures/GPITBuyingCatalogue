﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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
            Driver.FindElement(CommonSelectors.SubmitButton).Click();

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

        internal IEnumerable<string> GetTableRowCells(int cellIndex = 0)
        {
            return Driver.FindElements(CommonSelectors.TableRow)
                .Select(r => r.FindElements(CommonSelectors.TableCell)[cellIndex].Text);
        }

        internal string ClickCheckbox(By targetField, int index = 0)
        {
            Driver.FindElements(targetField)[index].FindElement(By.TagName("input")).Click();
            return Driver.FindElements(targetField)[index].FindElement(By.TagName("label")).Text;
        }

        internal void ClickCheckboxByLabel(string labelText)
        {
            var targetId =
            Driver
            .FindElements(By.ClassName("nhsuk-checkboxes__label"))
            .FirstOrDefault(label => label.Text == labelText)
            .GetAttribute("for");

            Driver.FindElement(By.Id(targetId)).Click();
        }

        internal void ClickRadioButtonWithValue(string value)
        {
            Driver.FindElements(CommonSelectors.RadioButtonItems)
                .Select(rb => rb.FindElement(By.TagName("input")))
                .Single(rbi => rbi.GetAttribute("value") == value)
                .Click();
        }

        internal string SelectDropDownItem(By targetField, int index = 0)
        {
            var selectElement = new SelectElement(Driver.FindElement(targetField));
            selectElement.SelectByIndex(index);

            return selectElement.SelectedOption.GetAttribute("value");
        }

        internal void SelectDropDownItemByValue(By targetField, string value)
        {
            var selectElement = new SelectElement(Driver.FindElement(targetField));
            selectElement.SelectByValue(value);
        }

        internal string SelectRandomDropDownItem(By targetField)
        {
            var selectElement = new SelectElement(Driver.FindElement(targetField));
            var optionsCount = selectElement.Options.Count;

            // Ignoring the zero index as that is a null value element (with text "Please select").
            return SelectDropDownItem(targetField, new Random().Next(1, optionsCount));
        }

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
            ElementIsDisplayed(CommonSelectors.SubmitButton);

        internal bool GoBackLinkDisplayed() =>
            ElementIsDisplayed(CommonSelectors.GoBackLink);

        internal bool ErrorSummaryDisplayed() =>
            ElementIsDisplayed(CommonSelectors.NhsErrorSection);

        internal bool ErrorSummaryLinksExist() =>
        Driver
        .FindElement(CommonSelectors.NhsErrorSectionLinkList)
        .FindElements(By.TagName("a"))
        .Select(l => l.GetAttribute("href"))
        .Select(s => s[(s.LastIndexOf('#') + 1)..])
        .All(href => ElementExists(By.Id(href)));

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

        internal int GetNumberOfCheckBoxesDisplayed() =>
            Driver.FindElements(By.ClassName("nhsuk-checkboxes__input")).Count;

        // Get Element Values
        internal string PageTitle() =>
            Driver.FindElement(CommonSelectors.Header1).Text.FormatForComparison();

        internal int GetNumberOfSelectedCheckBoxes() =>
            Driver
            .FindElements(By.ClassName("nhsuk-checkboxes__input"))
            .Count(cb => cb.Selected);

        internal bool CheckBoxSelectedByLabelText(string labelText)
        {
            var targetId =
            Driver
            .FindElements(By.ClassName("nhsuk-checkboxes__label"))
            .FirstOrDefault(label => label.Text == labelText)
            .GetAttribute("for");

            return Driver.FindElement(By.Id(targetId)).Selected;
        }

        internal bool AllCheckBoxesSelected() =>
            Driver
            .FindElements(By.ClassName("nhsuk-checkboxes__input"))
            .All(cb => cb.Selected);

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
            Driver.FindElement(targetElement).Text.EqualsIgnoreWhiteSpace(errorMessage);

        internal bool ElementTextEqualToo(By targetElement, string expectedText) =>
            Driver.FindElement(targetElement).Text.EqualsIgnoreWhiteSpace(expectedText);

        internal bool ElementTextContains(By targetElement, string expectedText) =>
            Driver.FindElement(targetElement).Text.Contains(expectedText);

        internal bool InputValueEqualToo(By targetElement, string expectedText) =>
            Driver.FindElement(targetElement).GetAttribute("value").EqualsIgnoreWhiteSpace(expectedText);

        internal bool InputElementIsEmpty(By targetElement) =>
            string.IsNullOrWhiteSpace(Driver.FindElement(targetElement).GetAttribute("value").FormatForComparison());

        // Element Utils
        internal bool PageLoadedCorrectGetIndex(
            Type controllerType,
            string methodName)
        {
            if (controllerType.BaseType != typeof(Controller))
                throw new InvalidOperationException($"{nameof(controllerType)} is not a type of {nameof(Controller)}");

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName), $"{nameof(methodName)} should not be null");

            Wait.Until(d => d.FindElement(CommonSelectors.NhsLogo));

            var controllerRoute = controllerType.GetCustomAttribute<RouteAttribute>(false)?.Template;

            var methodRoute = controllerType.GetMethods()
                .FirstOrDefault(m => m.Name == methodName && m.GetCustomAttribute<HttpGetAttribute>(false) is not null)?
                .GetCustomAttribute<HttpGetAttribute>(false)?.Template;

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
