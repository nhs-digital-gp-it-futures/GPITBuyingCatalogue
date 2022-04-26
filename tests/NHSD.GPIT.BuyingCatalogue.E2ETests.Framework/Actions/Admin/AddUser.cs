using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin
{
    public sealed class AddUser : ActionBase
    {
        public AddUser(IWebDriver driver)
            : base(driver)
        {
        }

        public void EnterFirstName(string firstName)
        {
            Driver.FindElement(Objects.Admin.AddUserObjects.FirstName).SendKeys(firstName);
        }

        public void EnterLastName(string lastName)
        {
            Driver.FindElement(Objects.Admin.AddUserObjects.LastName).SendKeys(lastName);
        }

        public void EnterEmailAddress(string emailAddress)
        {
            Driver.FindElement(Objects.Admin.AddUserObjects.Email).SendKeys(emailAddress);
        }

        public string GetConfirmationMessage()
        {
            return Driver.FindElement(Objects.Admin.AddUserObjects.ConfirmationTitle).Text;
        }
    }
}
