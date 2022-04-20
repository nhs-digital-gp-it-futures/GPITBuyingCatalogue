using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common
{
    public static class ByExtensions
    {
        public static By DataTestId(string dataTestId, string childCssSelector = null)
        {
            var selectorString = $"[data-test-id={dataTestId}] {childCssSelector}".Trim();

            return By.CssSelector(selectorString);
        }

        public static By DataValMessage(string valMessageContent)
        {
            var selectorString = $"[data-valmsg-for={valMessageContent}]".Trim();

            selectorString = selectorString.Replace(".", "\\.");

            return By.CssSelector(selectorString);
        }
    }
}
