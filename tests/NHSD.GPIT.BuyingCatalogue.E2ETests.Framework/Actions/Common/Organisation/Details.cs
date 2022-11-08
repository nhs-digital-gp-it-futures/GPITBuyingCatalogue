using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common.Organisation
{
    public sealed class Details : ActionBase
    {
        public Details(IWebDriver driver)
            : base(driver)
        {
        }
        public IEnumerable<string> GetAddress()
        {
            return Driver.FindElements(DetailsObjects.AddressLines).Select(s => s.Text);
        }

        public string GetOdsCode()
        {
            return Driver.FindElement(DetailsObjects.OdsCode).Text;
        }
    }
}
