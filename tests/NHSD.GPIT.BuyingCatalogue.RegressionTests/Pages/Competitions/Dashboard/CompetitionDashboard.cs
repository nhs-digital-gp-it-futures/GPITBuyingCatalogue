using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.Dashboard
{
    public class CompetitionDashboard : PageBase
    {
        public CompetitionDashboard(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CompetitionTriage()
        {
            CommonActions.LedeText()
            .Should()
            .Be("Create and manage orders, competitions and filters for your organisation.".FormatForComparison());

            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.CreateManageCompetitionLink);
            CommonActions.LedeText()
                .Should()
                .Be("Create new competitions or view and edit existing ones.".FormatForComparison());

            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.CreateCompetitionLink);
        }
    }
}
