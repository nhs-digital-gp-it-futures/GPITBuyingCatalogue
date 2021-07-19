using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
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
                Driver.FindElement(Objects.Ordering.OrderDescription.DescriptionInput);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool DescriptionInputShowingError(string errorMessage)
        {
            var errorSpanText = Driver.FindElement(ByExtensions.DataValMessage("Description")).Text;

            return errorSpanText == errorMessage;
        }
    }
}
