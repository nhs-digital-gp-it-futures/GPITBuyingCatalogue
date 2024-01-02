using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice
{
    public class ServiceLevelAgreement : PageBase
    {
        public ServiceLevelAgreement(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddServiceLevelAgreement()
        {
            CommonActions.ClickCheckboxByLabel("Service levels");
            CommonActions.ClickSave();
            CoreHours();
            ApplicableDays();
            BankHolidays();
        }

        public void AddServiceLevelAgreementForAllNonPriceElements()
        {
            CoreHours();
            ApplicableDays();
            BankHolidays();
        }

        public void CoreHours()
        {
            var timeFrom = DateTime.Now;
            var timeTo = DateTime.Now.AddHours(5);

            string slaTimeFrom = timeFrom.ToString("hh:mm");
            string slaTimeTo = timeTo.ToString("hh:mm");
            Driver.FindElement(NonPriceObjects.TimeFrom).SendKeys(slaTimeFrom.ToString());
            Driver.FindElement(NonPriceObjects.TimeUntil).SendKeys(slaTimeTo.ToString());
        }

        public void ApplicableDays()
        {
            CommonActions.ClickAllCheckboxes();
        }

        public void BankHolidays()
        {
            CommonActions.ClickRadioButtonWithText("Yes, include Bank Holidays");
            CommonActions.ClickSave();
        }
    }
}
