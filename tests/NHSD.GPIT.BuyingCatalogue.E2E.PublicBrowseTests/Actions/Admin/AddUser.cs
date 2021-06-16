using System;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal sealed class AddUser : ActionBase
    {
        public AddUser(IWebDriver driver) : base(driver)
        {
        }

        internal void EnterFirstName(string firstName)
        {
            Driver.FindElement(Objects.Admin.AddUserObjects.FirstName).SendKeys(firstName);
        }

        internal void EnterLastName(string lastName)
        {
            Driver.FindElement(Objects.Admin.AddUserObjects.LastName).SendKeys(lastName);
        }

        internal void EnterTelephoneNumber(string telephoneNumber)
        {
            Driver.FindElement(Objects.Admin.AddUserObjects.TelephoneNumber).SendKeys(telephoneNumber);
        }

        internal void EnterEmailAddress(string emailAddress)
        {
            Driver.FindElement(Objects.Admin.AddUserObjects.Email).SendKeys(emailAddress);
        }

        internal string GetConfirmationMessage()
        {
            return Driver.FindElement(Objects.Admin.AddUserObjects.ConfirmationTitle).Text;
        }

        internal void ClickAddUserButton()
        {
            Driver.FindElement(Objects.Admin.AddUserObjects.AddUserButton).Click();
        }
    }
}
