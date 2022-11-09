using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common.Organisation
{
    public sealed class AddUser : ActionBase
    {
        public AddUser(IWebDriver driver)
            : base(driver)
        {
        }

        public void EnterFirstName(string firstName)
        {
            Driver.FindElement(AddUserObjects.FirstName).SendKeys(firstName);
        }

        public void EnterLastName(string lastName)
        {
            Driver.FindElement(AddUserObjects.LastName).SendKeys(lastName);
        }

        public void EnterEmailAddress(string emailAddress)
        {
            Driver.FindElement(AddUserObjects.Email).SendKeys(emailAddress);
        }
        public void EnterRole(string role)
        {
            Driver.FindElements(CommonSelectors.RadioButtonItems)
                .First(r => r.FindElement(By.TagName("label")).Text == role)
                .FindElement(By.TagName("input"))
                .Click();
        }

        public string GetConfirmationMessage()
        {
            return Driver.FindElement(AddUserObjects.ConfirmationTitle).Text;
        }
    }
}
