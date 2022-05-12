using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    internal sealed class DetailsAndExpandersObjects
    {
        internal static By DetailsDropDownWithParagraph => By.XPath("//span[text()='This is a Details Dropdown With Some <p> text']");

        internal static By ExpanderDropdowns => By.CssSelector("[class='nhsuk-details__summary-text']");

        internal static By DetailsAndExpanders => By.XPath("//h1[text()='DetailsExpanders']");
    }
}
