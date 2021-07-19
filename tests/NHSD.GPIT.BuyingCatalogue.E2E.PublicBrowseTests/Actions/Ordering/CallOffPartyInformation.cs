using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Ordering
{
    internal sealed class CallOffPartyInformation : ActionBase
    {
        public CallOffPartyInformation(IWebDriver driver)
            : base(driver)
        {
        }

        internal bool OrganisationNameDisplayed()
        {
            try
            {
                return Driver.FindElement(Objects.Ordering.CalloffPartyInformation.OrganisationName).Displayed;
            }
            catch
            {
                return false;
            }
        }

        internal bool OrganisationOdsCodeDisplayed()
        {
            try
            {
                return Driver.FindElement(Objects.Ordering.CalloffPartyInformation.OrganisationOdsCode).Displayed;
            }
            catch
            {
                return false;
            }
        }

        internal bool OrganisationAddressFirstLineDisplayed()
        {
            try
            {
                return Driver.FindElement(Objects.Ordering.CalloffPartyInformation.OrganisationAddressLine1).Displayed;
            }
            catch
            {
                return false;
            }
        }

        internal bool FirstNameInputDisplayed()
        {
            try
            {
                return Driver.FindElement(Objects.Ordering.CalloffPartyInformation.FirstNameInput).Displayed;
            }
            catch
            {
                return false;
            }
        }

        internal bool LastNameInputDisplayed()
        {
            try
            {
                return Driver.FindElement(Objects.Ordering.CalloffPartyInformation.LastNameInput).Displayed;
            }
            catch
            {
                return false;
            }
        }

        internal bool EmailAddressInputDisplayed()
        {
            try
            {
                return Driver.FindElement(Objects.Ordering.CalloffPartyInformation.EmailAddressInput).Displayed;
            }
            catch
            {
                return false;
            }
        }

        internal bool PhoneNumberInputDisplayed()
        {
            try
            {
                return Driver.FindElement(Objects.Ordering.CalloffPartyInformation.PhoneNumberInput).Displayed;
            }
            catch
            {
                return false;
            }
        }
    }
}
