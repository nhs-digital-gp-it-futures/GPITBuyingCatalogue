using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    internal sealed class CheckBoxesObjects
    {
        internal static By SingleCheckBoxProperty => By.Id("SingleCheckBoxProperty");

        internal static By AnotherCheckBoxProperty => By.Id("AnotherCheckBoxProperty");

        internal static By EmbeddedCheckBoxProperty => By.Id("EmbeddedCheckBoxProperty");

        internal static By ConditionalSingleCheckBoxProperty => By.CssSelector("[aria-controls='conditional-SingleCheckBoxProperty']");

        internal static By ConditionalAnotherCheckBoxProperty => By.CssSelector("[aria-controls='conditional-AnotherCheckBoxProperty']");

        internal static By ForInput => By.Id("ForInput");
    }
}
