using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions
{
    public class SolutionServiceLevelAgreement : PageBase
    {
        public SolutionServiceLevelAgreement(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddServiceLevelAgreement(string solutionId)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.ServiceLevelAgreementLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddServiceLevelAgreement))
                .Should().BeTrue();

            AddSlaType();

            CommonActions.ClickSaveAndContinue();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }

        public void AddSlaType()
        {
            CommonActions.ClickRadioButtonWithText("Type 1 Catalogue Solution");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should().BeTrue();

            AddAvailabilityTimes();
            AddContactLevelDetails();
            AddServiceLevelDetails();
        }

        private void AddAvailabilityTimes()
        {
            CommonActions.ClickLinkElement(ServiceAvailabilityTimesObjects.AddAvailabilityTimesLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddServiceAvailabilityTimes))
                .Should().BeTrue();

            var currentDate = DateTime.UtcNow;

            var fromTime = currentDate.AddMinutes(-5).ToString("HH:mm");
            var untilTime = currentDate.ToString("HH:mm");

            TextGenerators.TextInputAddText(ServiceAvailabilityTimesObjects.SupportTypeInput, 20);
            CommonActions.ClickAllCheckboxes();

            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.FromInput, fromTime);
            CommonActions.ElementAddValue(ServiceAvailabilityTimesObjects.UntilInput, untilTime);

            CommonActions.ClickRadioButtonWithValue(false.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }

        private void AddContactLevelDetails()
        {
            const string timefrom = "12:30";
            const string timeUntil = "13:30";

            CommonActions.ClickLinkElement(SLAContactObjects.AddContactLevelDetailsLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddContact))
                .Should().BeTrue();

            TextGenerators.TextInputAddText(SLAContactObjects.Channel, 50);
            TextGenerators.TextInputAddText(SLAContactObjects.ContactInformation, 250);
            CommonActions.ElementAddValue(SLAContactObjects.From, timefrom);
            CommonActions.ElementAddValue(SLAContactObjects.Until, timeUntil);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }

        private void AddServiceLevelDetails()
        {
            CommonActions.ClickLinkElement(ServiceLevelObjects.AddServiceLevelLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.AddServiceLevel))
                .Should().BeTrue();

            TextGenerators.TextInputAddText(ServiceLevelObjects.ServiceTypeInput, 20);
            TextGenerators.TextInputAddText(ServiceLevelObjects.ServiceLevelInput, 20);
            TextGenerators.TextInputAddText(ServiceLevelObjects.HowMeasuredInput, 20);

            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ServiceLevelAgreementsController),
                nameof(ServiceLevelAgreementsController.EditServiceLevelAgreement))
                .Should()
                .BeTrue();
        }
    }
}
