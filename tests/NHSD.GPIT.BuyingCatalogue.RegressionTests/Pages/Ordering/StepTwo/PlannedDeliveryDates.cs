using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo
{
    public class PlannedDeliveryDates : PageBase
    {
        public PlannedDeliveryDates(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void PlannedDeliveryDate(string solutionName, bool isAssociatedServiceOnly, IEnumerable<string>? associatedServices, IEnumerable<string>? additionalServices)
        {
            SetDefaultPlannedDeliveryDate(DateTime.Today.AddDays(7));
            CommonActions.ClickSave();

            var names = SelectSolutionAndServices.SelectSolutionServices(solutionName, isAssociatedServiceOnly, associatedServices, additionalServices);

            foreach (var name in names)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

                CommonActions.ClickSave();
            }

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.Review)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        public void EditPlannedDeliveryDate(string solutionName, bool isAssociatedServiceOnly, string associatedServices, string additionalServices, bool editplanneddeliverydate)
        {
            CommonActions.ClickLinkElement(DeliveryDatesObjects.ReviewChangeDeliveryDateLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

            SetDefaultPlannedDeliveryDate(DateTime.Today.AddDays(8));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.ConfirmChanges)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText(ConfirmChangesModel.YesOption);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            EditDefaultPlannedDeliveryDate(DateTime.Today.AddDays(9));
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.MatchDates)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText(MatchDatesModel.YesOption);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.MatchDates)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText(MatchDatesModel.YesOption);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.Review)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

        }

        public void SetDefaultPlannedDeliveryDate(DateTime date)
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, $"{date.Day}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, $"{date.Month}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, $"{date.Year}");
        }

        public void EditDefaultPlannedDeliveryDate(DateTime date)
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesDayInput(0), $"{date.Day}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesMonthInput(0), $"{date.Month}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.EditDatesYearInput(0), $"{date.Year}");
        }
    }
}
