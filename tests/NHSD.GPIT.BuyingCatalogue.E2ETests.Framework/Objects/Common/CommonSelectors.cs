using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common
{
    public static class CommonSelectors
    {
        public static By SubmitButton => By.Id("Submit");

        public static By CancelLink => By.LinkText("Cancel");

        public static By SelectAll => By.XPath("//a[contains(@href, 'select-recipients?selectionMode=All')]");

        public static By Header1 => By.TagName("h1");

        public static By Header2 => By.TagName("h2");

        public static By Header3 => By.TagName("h3");

        public static By GoBackLink => By.ClassName("nhsuk-back-link__link");

        public static By RadioButtonItems => By.CssSelector(".nhsuk-radios__item");

        public static By RadioButtonInputs => By.CssSelector(".nhsuk-radios__input");

        public static By RadioButtons => By.ClassName("nhsuk-radios");

        public static By DropDownList => By.Id("SelectedFilterId");

        public static By DropDownItem => By.Id("SelectedOrderItemId");

        public static By BreadcrumbList => By.ClassName("nhsuk-breadcrumb__list");

        public static By BreadcrumbItem => By.ClassName("nhsuk-breadcrumb__item");

        public static By CheckboxItem => By.CssSelector(".nhsuk-checkboxes__item");

        public static By NhsErrorSection => By.ClassName("nhsuk-error-summary");

        public static By NhsErrorSectionLinkList => By.ClassName("nhsuk-error-summary__list");

        public static By NhsInsetText => By.ClassName("nhsuk-inset-text");

        public static By LinkTextBox => By.Id("Link");

        public static By TableRow => By.CssSelector("tbody tr.nhsuk-table__row");

        public static By TableCell => By.CssSelector("td.nhsuk-table__cell");

        public static By AdditionalInfoTextArea => By.Id("AdditionalInformation");

        public static By Description => By.Id("Description");

        public static By DateDay => By.Id("Day");

        public static By DateMonth => By.Id("Month");

        public static By DateYear => By.Id("Year");

        public static By ActionLink => By.ClassName("nhsuk-action-link__link");

        public static By Pagination => By.CssSelector("nav.nhsuk-pagination");

        public static By PaginationPrevious => By.ClassName("nhsuk-pagination__link--prev");

        public static By PaginationNext => By.ClassName("nhsuk-pagination__link--next");

        public static By PaginationPreviousSubText => By.CssSelector("a.nhsuk-pagination__link--prev span.nhsuk-pagination__page");

        public static By PaginationNextSubText => By.CssSelector("a.nhsuk-pagination__link--next span.nhsuk-pagination__page");

        public static By Name => By.Id("Name");

        public static By OrderGuidance => By.Id("OrderGuidance");

        public static By ContinueButton => By.LinkText("Continue");

        public static By SaveAndContinue => By.LinkText("Save and continue");

        public static By PriceDetailsHelp => ByExtensions.DataTestId("price-details");
    }
}
