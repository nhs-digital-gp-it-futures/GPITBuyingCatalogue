using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common
{
    internal static class ByExtensions
    {
        internal static By DataTestId(string dataTestId, string childCssSelector = null)
        {
            var selectorString = $"[data-test-id={dataTestId}] {childCssSelector}".Trim();

            return By.CssSelector(selectorString);
        }

        internal static By DataValMessage(string valMessageContent)
        {
            var selectorString = $"[data-valmsg-for={valMessageContent}]".Trim();

            return By.CssSelector(selectorString);
        }
    }
}
