using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
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
    }
}
