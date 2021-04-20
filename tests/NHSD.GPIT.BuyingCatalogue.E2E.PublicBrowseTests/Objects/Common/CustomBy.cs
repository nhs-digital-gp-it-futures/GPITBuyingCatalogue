using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common
{
    internal sealed class CustomBy : By
    {
        internal static By DataTestId(string dataTestId, string childCssSelector = null)
        {
            var selectorString = $"[data-test-id={dataTestId}] {childCssSelector}".Trim();

            return CssSelector(selectorString);
        }
    }
}
