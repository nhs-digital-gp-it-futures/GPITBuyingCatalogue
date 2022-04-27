using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.Extensions
{
    internal static class ElementExtensions
    {
        public static bool ContainsElement(this IWebElement element, By by)
        {
            try
            {
                element.FindElement(by);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
