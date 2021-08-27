using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common
{
    internal static class CommonSelectors
    {
        internal static By SubmitButton => By.Id("Submit");

        internal static By Header1 => By.TagName("h1");

        internal static By Header3 => By.TagName("h3");

        internal static By GoBackLink => By.ClassName("nhsuk-back-link__link");

        internal static By RadioButtonItems => By.CssSelector(".nhsuk-radios__item");

        internal static By RadioButtons => By.ClassName("nhsuk-radios");

        internal static By CheckboxItem => By.CssSelector(".nhsuk-checkboxes__item");

        internal static By NhsErrorSection => By.ClassName("nhsuk-error-summary");

        internal static By NhsErrorSectionLinkList => By.ClassName("nhsuk-error-summary__list");

        internal static By LinkTextBox => By.Id("Link");

        internal static By TableRow => By.CssSelector("tbody tr.nhsuk-table__row");

        internal static By TableCell => By.CssSelector("td.nhsuk-table__cell");

        internal static By AdditionalInfoTextArea => By.Id("AdditionalInformation");

        internal static By Description => By.Id("Description");

        internal static By DateDay => By.Id("Day");

        internal static By DateMonth => By.Id("Month");

        internal static By DateYear => By.Id("Year");

        internal static By ActionLink => By.ClassName("nhsuk-action-link__link");

        internal static By Pagination => By.CssSelector("nav.nhsuk-pagination");

        internal static By PaginationPrevious => By.ClassName("nhsuk-pagination__link--prev");

        internal static By PaginationNext => By.ClassName("nhsuk-pagination__link--next");

        internal static By PaginationPreviousSubText => By.CssSelector("a.nhsuk-pagination__link--prev span.nhsuk-pagination__page");

        internal static By PaginationNextSubText => By.CssSelector("a.nhsuk-pagination__link--next span.nhsuk-pagination__page");
    }
}
