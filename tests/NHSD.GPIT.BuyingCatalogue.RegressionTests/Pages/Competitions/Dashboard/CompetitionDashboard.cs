using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.Dashboard
{
    internal class CompetitionDashboard : PageBase
    {
        public CompetitionDashboard(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CreateNewCompetition()
        {
            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.CreateManageCompetitionLink);

            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.CreateCompetitionLink);

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(CompetitionsDashboardController),
               nameof(CompetitionsDashboardController.BeforeYouStart))
           .Should()
           .BeTrue();
        }
    }
}
