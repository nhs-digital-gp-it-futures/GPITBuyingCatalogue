using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.DeliveryDates
{
    public class PlannedDeliveryDates : PageBase
    {
        public PlannedDeliveryDates(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void PlannedDeliveryDate(string solutionName, bool isAssociatedServiceOnly, IEnumerable<string>? associatedServices, IEnumerable<string>? additionalServices)
        {
            SetDefaultPlannedDeliveryDate(DateTime.Today.AddDays(7));
            CommonActions.ClickRadioButtonWithText("No");
            CommonActions.ClickSave();

            var names = SelectSolutionAndServices.SelectSolutionServices(solutionName, isAssociatedServiceOnly, associatedServices, additionalServices);

            foreach (var name in names)
            {
                EditDatePlannedDeliveryDate();
            }

            ReviewPlannedDeliveryDate();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        public void AmendPlannedDeliveryDate(string name)
        {
            SetDefaultPlannedDeliveryDate(DateTime.Today.AddDays(7));
            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(DeliveryDatesController),
            nameof(DeliveryDatesController.Review)).Should().BeTrue();
            CommonActions.ClickContinue();
        }

        public void AmendEditPlannedDeliveryDate(string name)
        {
            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(DeliveryDatesController),
                    nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ClickSave();
        }

        public void EditPlannedDeliveryDate(string solutionName, bool isAssociatedServiceOnly, IEnumerable<string>? associatedServices, IEnumerable<string>? additionalServices)
        {
            var names = SelectSolutionAndServices.SelectSolutionServices(solutionName, isAssociatedServiceOnly, associatedServices, additionalServices);
            if (isAssociatedServiceOnly)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                        typeof(DeliveryDatesController),
                        nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();
                CommonActions.ClickRadioButtonWithText("No");
                CommonActions.ClickSave();
                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(DeliveryDatesController),
                    nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

                foreach (var name in names)
                {
                    EditDefaultPlannedDeliveryDate(DateTime.Today.AddDays(9));
                    CommonActions.ClickSave();
                }

                ReviewPlannedDeliveryDate();
                CommonActions.PageLoadedCorrectGetIndex(
                        typeof(OrderController),
                        nameof(OrderController.Order)).Should().BeTrue();
            }
            else
            {
                CommonActions.ClickRadioButtonWithText("Yes");

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.Review)).Should().BeTrue();
                CommonActions.ClickContinue();
            }
        }

        public void EditPlannedDeliveryDate(string solutionName, bool isAssociatedServiceOnly, string associatedServices, string additionalServices, bool editplanneddeliverydate = false)
        {
            if (!isAssociatedServiceOnly)
            {
                CommonActions.ClickLinkElement(DeliveryDatesObjects.ReviewChangeDeliveryDateLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(DeliveryDatesController),
                    nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

                SetDefaultPlannedDeliveryDate(DateTime.Today.AddDays(8));
                CommonActions.ClickRadioButtonWithText("No");
                ConfirmPlannedDeliveryPageYesOption();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(DeliveryDatesController),
                    nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

                EditDefaultPlannedDeliveryDate(DateTime.Today.AddDays(9));
                CommonActions.ClickSave();

                var names = SelectSolutionAndServices.EditPlannedDeliveryDateSelectSolutionServices(isAssociatedServiceOnly, associatedServices, additionalServices);

                foreach (var name in names)
                {
                    MatchDatesPlannedDeliveryDate();
                    EditDatePlannedDeliveryDate();
                }

                ReviewPlannedDeliveryDate();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrderController),
                    nameof(OrderController.Order)).Should().BeTrue();
            }
            else
            {
                CommonActions.ClickLinkElement(DeliveryDatesObjects.ReviewChangeDeliveryDateLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(DeliveryDatesController),
                    nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

                SetDefaultPlannedDeliveryDate(DateTime.Today.AddDays(8));
                CommonActions.ClickRadioButtonWithText("No");
                ConfirmPlannedDeliveryPageYesOption();

                EditDatePlannedDeliveryDate();
                ReviewPlannedDeliveryDate();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrderController),
                    nameof(OrderController.Order)).Should().BeTrue();
            }
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

        public void ConfirmPlannedDeliveryPageYesOption()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.ConfirmChanges)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText(ConfirmChangesModel.YesOption);
            CommonActions.ClickSave();
        }

        public void ReviewPlannedDeliveryDate()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.Review)).Should().BeTrue();

            CommonActions.ClickContinue();
        }

        public void MatchDatesPlannedDeliveryDate()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.MatchDates)).Should().BeTrue();

            CommonActions.ClickRadioButtonWithText(MatchDatesModel.YesOption);
            CommonActions.ClickSave();
        }

        public void EditDatePlannedDeliveryDate()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            CommonActions.ClickSave();
        }
    }
}
