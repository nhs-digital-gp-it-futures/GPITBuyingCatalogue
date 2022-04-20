using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Marketing
{
    public class ContactDetailsActions : ActionBase
    {
        public ContactDetailsActions(IWebDriver driver)
            : base(driver)
        {
        }

        public void AddMarketingContact(MarketingContact contact, int index = 1)
        {
            Driver.FindElement(ContactDetailsObjects.ContactFirstName(index)).Clear();
            Driver.FindElement(ContactDetailsObjects.ContactLastName(index)).Clear();
            Driver.FindElement(ContactDetailsObjects.ContactEmailAddress(index)).Clear();
            Driver.FindElement(ContactDetailsObjects.ContactPhoneNumber(index)).Clear();
            Driver.FindElement(ContactDetailsObjects.ContactJobSector(index)).Clear();

            Driver.FindElement(ContactDetailsObjects.ContactFirstName(index)).SendKeys(contact.FirstName);
            Driver.FindElement(ContactDetailsObjects.ContactLastName(index)).SendKeys(contact.LastName);
            Driver.FindElement(ContactDetailsObjects.ContactEmailAddress(index)).SendKeys(contact.Email);
            Driver.FindElement(ContactDetailsObjects.ContactPhoneNumber(index)).SendKeys(contact.PhoneNumber);
            Driver.FindElement(ContactDetailsObjects.ContactJobSector(index)).SendKeys(contact.Department);
        }
    }
}
