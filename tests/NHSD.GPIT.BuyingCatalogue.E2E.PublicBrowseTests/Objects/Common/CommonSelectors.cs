using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common
{
    internal static class CommonSelectors
    {
        internal static By SubmitButton => By.Id("Submit");

        internal static By Header1 => By.TagName("h1");

        internal static By GoBackLink => By.ClassName("nhsuk-back-link__link");

        internal static By RadioButtonItems => By.CssSelector(".nhsuk-radios__item");

        internal static By RadioButtons => By.ClassName("nhsuk-radios");

        internal static By CheckboxItem => By.CssSelector(".nhsuk-checkboxes__item");

        internal static By NhsErrorSection => By.ClassName("nhsuk-error-summary");

        internal static By LinkTextBox => By.Id("Link");

        internal static By TableRow => By.CssSelector("tbody tr.nhsuk-table__row");

        internal static By TableCell => By.CssSelector("td.nhsuk-table__cell");

        internal static By AdditionalInfoTextArea => By.Id("AdditionalInformation");
    }
}
