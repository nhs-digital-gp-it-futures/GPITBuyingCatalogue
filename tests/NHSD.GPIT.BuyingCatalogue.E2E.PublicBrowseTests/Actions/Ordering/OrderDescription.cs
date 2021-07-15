using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Ordering
{
    internal sealed class OrderDescription : ActionBase
    {
        public OrderDescription(IWebDriver driver)
            : base(driver)
        {
        }

        internal bool DescriptionInputDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElement(Objects.Ordering.OrderDescription.DescriptionInput));
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool DescriptionInputShowingError(string errorMessage)
        {
            var errorSpanText = Driver.FindElement(By.XPath("//span[@data-valmsg-for=\"Description\"")).Text;

            return errorSpanText == errorMessage;
        }
    }
}
