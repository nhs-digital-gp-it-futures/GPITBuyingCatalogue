using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
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
            CommonActions.LedeText().Should().Be("This is what you're ordering based on this competition.".FormatForComparison());

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
    }
}
