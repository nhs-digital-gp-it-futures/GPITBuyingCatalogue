using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common
{
    public sealed class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver)
            : base(driver)
        {
        }

        // Click or Select Actions
        public void ClickGoBackLink() =>
            Driver.FindElement(CommonSelectors.GoBackLink).Click();

        public void ClickCancel() =>
            Driver.FindElement(By.LinkText("Cancel")).Click();

        public void ClickAddBespokeMilestone() =>
            Driver.FindElement(By.ClassName("nhsuk-action-link__link")).Click();

        public void ClickStartNewSearch() =>
            Driver.FindElement(By.LinkText("Start a new search")).Click();

        public void ClickContinue() =>
            Driver.FindElement(CommonSelectors.ContinueButton).Click();

        public void ClickLinkElement(By targetElement) =>
            Driver.FindElement(targetElement).Click();

        public void ClickLinkElement(By targetElement, string hrefContains)
        {
            Driver.FindElements(targetElement)
                .First(s => s.GetAttribute("href").Contains(hrefContains))
                .Click();
        }

        public void ClickSave() =>
            Driver.FindElement(CommonSelectors.SubmitButton).Click();

        public void ClickFirstCheckbox() =>
            Driver.FindElements(By.CssSelector("input[type=checkbox]")).First().Click();

        public void ClickFirstRadio() =>
            Driver.FindElements(By.CssSelector("input[type=radio]")).First().Click();

        public void ClickLastRadio() =>
            Driver.FindElements(By.CssSelector("input[type=radio]")).Last().Click();

        public void ClickFirstUnselectedRadio() =>
            Driver.FindElements(CommonSelectors.RadioButtonInputs)
            .Where(r => !r.Selected).First().Click();

        public void ClickAllCheckboxes() =>
            Driver.FindElements(By.CssSelector("input[type=checkbox]")).ToList().ForEach(element => element.Click());

        public void ClickMultipleCheckboxes(int multipleServiceRecipients) =>
            Driver.FindElements(By.CssSelector("input[type=checkbox]")).ToList().Take(multipleServiceRecipients).ForEach(element => element.Click());

        public void UncheckAllCheckboxes() =>
            Driver.FindElements(By.CssSelector("input[type=checkbox]"))
                .Where(x => x.GetAttribute("checked") != null)
                .ToList()
                .ForEach(x => x.Click());

        public void ClickSection(By targetField, string section) =>
            Driver.FindElements(targetField)
                .First(s => s.Text.Contains(section)).FindElement(By.TagName("a"))
                .Click();

        public void ClickRadioButtonWithText(string label) =>
            Driver.FindElements(CommonSelectors.RadioButtonItems)
                .First(r => r.FindElement(By.TagName("label")).Text == label)
                .FindElement(By.TagName("input"))
                .Click();

        public void ClickDropDownListWIthValue(int value) =>
            new SelectElement(
            Driver.FindElement(CommonSelectors.DropDownList)).SelectByIndex(value);

        public void EnterTextInTextBoxes(string x) =>
            Driver.FindElements(By.XPath("//*[@class=\"nhsuk-textarea\"]")).ToList().ForEach(element => element.SendKeys(x));


        public IEnumerable<string> GetRadioButtonsOptions() =>
            Driver
                .FindElements(CommonSelectors.RadioButtonItems)
                .Select(e => e.Text);

        public IEnumerable<string> GetRadioButtonsOptionsIds() =>
            Driver
            .FindElements(CommonSelectors.RadioButtonInputs)
            .Select(e => e.GetAttribute("Value"));

        public bool IsRadioButtonChecked(string radioButtonValue) =>
            Driver
                .FindElements(CommonSelectors.RadioButtonInputs)
                .Where(r => r.GetAttribute("Value") == radioButtonValue)
                .Any(r => r.Selected);

        public IEnumerable<string> GetTableRowCells(int cellIndex = 0)
        {
            return Driver.FindElements(CommonSelectors.TableRow)
                .Select(r => r.FindElements(CommonSelectors.TableCell)[cellIndex].Text);
        }

        public string ClickCheckbox(By targetField, int index = 0)
        {
            Driver.FindElements(targetField)[index].FindElement(By.TagName("input")).Click();
            return Driver.FindElements(targetField)[index].FindElement(By.TagName("label")).Text;
        }

        public void ClickCheckboxByLabel(string labelText)
        {
            var targetId =
            Driver
            .FindElements(By.ClassName("nhsuk-checkboxes__label"))
            .FirstOrDefault(label => label.Text == labelText)
            .GetAttribute("for");

            Driver.FindElement(By.Id(targetId)).Click();
        }

        public void ClickRadioButtonWithValue(string value)
        {
            Driver.FindElements(CommonSelectors.RadioButtonItems)
                .Select(rb => rb.FindElement(By.TagName("input")))
                .First(rbi => rbi.GetAttribute("value") == value)
                .Click();
        }

        public void ClickRadioButtonWithValue(By parent, string value)
        {
            Driver.FindElements(parent)
                .First(rbi => rbi.GetAttribute("value") == value)
                .Click();
        }

        public string SelectDropDownItem(By targetField, int index = 0)
        {
            var selectElement = new SelectElement(Driver.FindElement(targetField));
            selectElement.SelectByIndex(index);

            return selectElement.SelectedOption.GetAttribute("value");
        }

        public void SelectDropDownItemByValue(By targetField, string value)
        {
            var selectElement = new SelectElement(Driver.FindElement(targetField));
            selectElement.SelectByValue(value);
        }

        public string SelectRandomDropDownItem(By targetField)
        {
            var selectElement = new SelectElement(Driver.FindElement(targetField));
            var optionsCount = selectElement.Options.Count;

            // Ignoring the zero index as that is a null value element (with text "Please select").
            return SelectDropDownItem(targetField, new Random().Next(1, optionsCount));
        }

        public string GetRecipientImportCsv(string fileName)
            => Path.GetFullPath(Path.Combine("ServiceRecipientTestData", fileName));

        // Input Element Actions
        public void UploadFile(By targetElement, string filePath)
            => Driver.FindElement(targetElement).SendKeys(filePath);

        public void ClearInputElement(By targetElement) =>
            Driver.FindElement(targetElement).Clear();

        public void AutoCompleteAddValue(By targetElement, string value)
        {
            Driver.FindElement(targetElement).Click();
            Driver.FindElement(targetElement).SendKeys(value);
        }

        public void ElementAddValue(By targetElement, string value)
        {
            Driver.FindElement(targetElement).Clear();
            Driver.FindElement(targetElement).SendKeys(value);
        }

        public async Task ElementAddValueWithDelay(By targetElement, string value, int milliseconds = 2500)
        {
            ElementAddValue(targetElement, value);
            await Task.Delay(milliseconds);
        }

        // Element Displayed
        public bool ElementIsDisplayed(By targetElement) =>
            ElementExists(targetElement) && Driver.FindElement(targetElement).Displayed;

        public bool ElementIsNotDisplayed(By targetElement) =>
            ElementExists(targetElement) && !Driver.FindElement(targetElement).Displayed;

        public int NumberOfElementsDisplayed(By targetElement) =>
            ElementExists(targetElement) ? Driver.FindElements(targetElement).Count : 0;

        public bool SaveButtonDisplayed() =>
            ElementIsDisplayed(CommonSelectors.SubmitButton);

        public bool ContinueButtonDisplayed() =>
            ElementIsDisplayed(CommonSelectors.ContinueButton);

        public bool GoBackLinkDisplayed() =>
            ElementIsDisplayed(CommonSelectors.GoBackLink);

        public bool ErrorSummaryDisplayed() =>
            ElementIsDisplayed(CommonSelectors.NhsErrorSection);

        public bool CancelLinkDisplayed() =>
            ElementIsDisplayed(CommonSelectors.CancelLink);

        public bool ErrorSummaryLinksExist() =>
        Driver
        .FindElement(CommonSelectors.NhsErrorSectionLinkList)
        .FindElements(By.TagName("a"))
        .Select(l => l.GetAttribute("href"))
        .Select(s => s[(s.LastIndexOf('#') + 1)..])
        .All(href => ElementExists(By.Id(href)));

        public bool ElementExists(By targetElement)
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

        public int GetNumberOfRadioButtonsDisplayed() =>
            Driver.FindElements(By.ClassName("nhsuk-radios__input")).Count;

        public int GetNumberOfCheckBoxesDisplayed() =>
            Driver.FindElements(By.ClassName("nhsuk-checkboxes__input")).Count;

        public int GetNumberOfTableRowsDisplayed() =>
            Driver.FindElements(By.ClassName("nhsuk-table__row")).Count;

        // Get Element Values
        public string PageTitle() =>
            Driver.FindElement(CommonSelectors.Header1).Text.FormatForComparison();

        public string LedeText() =>
            Driver.FindElement(By.ClassName("nhsuk-lede-text")).Text.FormatForComparison();

        public int GetNumberOfSelectedCheckBoxes() =>
            Driver
            .FindElements(By.ClassName("nhsuk-checkboxes__input"))
            .Count(cb => cb.Selected);

        public int GetNumberOfSelectedRadioButtons() => Driver
            .FindElements(By.ClassName("nhsuk-radios__input"))
            .Count(x => x.Selected);

        public bool CheckBoxSelectedByLabelText(string labelText)
        {
            var targetId =
            Driver
            .FindElements(By.ClassName("nhsuk-checkboxes__label"))
            .FirstOrDefault(label => label.Text == labelText)
            .GetAttribute("for");

            return Driver.FindElement(By.Id(targetId)).Selected;
        }

        public bool AllCheckBoxesSelected() =>
            Driver
            .FindElements(By.ClassName("nhsuk-checkboxes__input"))
            .All(cb => cb.Selected);

        public bool AnyCheckboxSelected() =>
            Driver
                .FindElements(By.CssSelector("input[type=checkbox]"))
                    .Any(cb => cb.Selected);    

        // Element Comparisons

        /// <summary>
        /// Returns if the referenced Element's Error message is equal to the Expected Error Message.
        /// </summary>
        /// <param name="dataValMessage">This will be the value of data-valmgs-for on the Error span when the elements is in error.</param>
        /// <param name="errorMessage">the expected error message.</param>
        /// <returns>true if error message is same as expected, false if not.</returns>
        public bool ElementShowingCorrectErrorMessage(string dataValMessage, string errorMessage) =>
            ElementShowingCorrectErrorMessage(ByExtensions.DataValMessage(dataValMessage), errorMessage);

        public bool ElementShowingCorrectErrorMessage(By targetElement, string errorMessage) =>
            Driver.FindElement(targetElement).Text.EqualsIgnoreWhiteSpace(errorMessage);

        public bool ElementTextEqualTo(By targetElement, string expectedText) =>
            Driver.FindElement(targetElement).Text.EqualsIgnoreWhiteSpace(expectedText);

        public bool ElementTextContains(By targetElement, string expectedText) =>
            Driver.FindElement(targetElement).Text.Contains(expectedText);

        public bool InputValueEqualTo(By targetElement, string expectedText) =>
            Driver.FindElement(targetElement).GetAttribute("value").EqualsIgnoreWhiteSpace(expectedText);

        public bool InputElementIsEmpty(By targetElement) =>
            string.IsNullOrWhiteSpace(Driver.FindElement(targetElement).GetAttribute("value").FormatForComparison());

        // Element Utils
        public bool PageLoadedCorrectGetIndex(
            Type controllerType,
            string methodName)
        {
            if (!typeof(Controller).IsAssignableFrom(controllerType))
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
                .Where((t, i) => !t.StartsWith("%7B") && driverUrl.Segments[i].ToLower() != t.ToLower())
                .Any();
        }

        public void WaitUntilElementExists(By element) => Wait.Until(d => d.FindElement(element));

        public void WaitUntilElementIsDisplayed(By element) => Wait.Until(d => d.FindElement(element).Displayed);

        public async Task InputCharactersWithDelay(By inputBox, string input, int intervalMs = 200)
        {
            foreach (var character in input)
            {
                ElementAddValue(inputBox, $"{character}");
                await Task.Delay(intervalMs);
            }
        }
    }
}
