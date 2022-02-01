using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Actions
{
    internal class EndnoteActions : ActionBase
    {
        public EndnoteActions(IWebDriver driver) : base(driver)
        {
        }

        public string GetEndnotCode()
        {
            return Driver.FindElement(EndNoteObjects.CodeExampleForEndnote).Text;
        }
    }
}
