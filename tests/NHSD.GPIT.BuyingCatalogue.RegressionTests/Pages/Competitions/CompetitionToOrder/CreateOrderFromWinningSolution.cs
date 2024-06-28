using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.FundingSource;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.CompetitionToOrder
{
    public class CreateOrderFromWinningSolution : PageBase
    {
        public CreateOrderFromWinningSolution(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CreateOrderFromCompetition(string winningsolution)
        {
            CommonActions.ClickLinkElement(CompetitionToOrderObjects.CreateOrderFromeCompetitionWinningSolution(winningsolution));
            CommonActions.HintText().Should().Be("This is what you're ordering based on this competition.".FormatForComparison());

            CommonActions.ClickSave();
        }

        public void ConfirmContact()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(OrderController),
               nameof(OrderController.Order)).Should().BeTrue();
        }

        public void AddTimescaleForCallOffAgreement()
        {
            TextGenerators.DateInputAddDateSoon(
                CommencementDateObjects.CommencementDateDayInput,
                CommencementDateObjects.CommencementDateMonthInput,
                CommencementDateObjects.CommencementDateYearInput);

            var initialPeriod = TextGenerators.NumberInputAddRandomNumber(CommencementDateObjects.InitialPeriodInput, 1, 6);

            CommonActions.ClickSave();
        }

        public void SetDefaultPlannedDeliveryDate(DateTime date)
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, $"{date.Day}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, $"{date.Month}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, $"{date.Year}");
        }

        public void ReviewPlannedDeliveryDate()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.Review)).Should().BeTrue();

            CommonActions.ClickSaveAndContinue();
        }

        public void CompetitionPlannedDeliveryDate()
        {
            SetDefaultPlannedDeliveryDate(DateTime.Today.AddDays(7));
            CommonActions.ClickRadioButtonWithText("Yes");
            CommonActions.ClickSave();
            ReviewPlannedDeliveryDate();
        }

        public void FundingSources(IEnumerable<CatalogueItemId> fundingservices)
        {
            foreach (var service in fundingservices)
            {
                CommonActions.ClickLinkElement(CompetitionToOrderObjects.FundingSources(service.ToString()));

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.FundingSource))
                    .Should().BeTrue();

                CommonActions.ClickFirstRadio();

                CommonActions.ClickSave();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.FundingSources))
                    .Should().BeTrue();
            }

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(OrderController),
              nameof(OrderController.Order)).Should().BeTrue();
        }

        public void FundingSource(CatalogueItemId solutionId)
        {
            CommonActions.ClickLinkElement(CompetitionToOrderObjects.FundingSources(solutionId.ToString()));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSource))
                .Should().BeTrue();

            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSources))
                .Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(OrderController),
              nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
