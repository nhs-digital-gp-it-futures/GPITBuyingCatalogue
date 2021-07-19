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
                Driver.FindElement(Objects.Ordering.OrderDescription.DescriptionInput);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal string DescriptionInputValue()
        {
            try
            {
                var element = Driver.FindElement(Objects.Ordering.OrderDescription.DescriptionInput);
                return element.Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal void DeleteDescriptionInputValue()
        {
            Driver.FindElement(Objects.Ordering.OrderDescription.DescriptionInput).Clear();
        }
    }
}
