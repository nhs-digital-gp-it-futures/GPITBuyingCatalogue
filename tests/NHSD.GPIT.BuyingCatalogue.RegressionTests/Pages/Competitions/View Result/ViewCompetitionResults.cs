using FluentAssertions;
using Microsoft.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.View_Result
{
    public class ViewCompetitionResults : PageBase
    {
        public ViewCompetitionResults(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void ViewResults()
        {
            CommonActions.ClickSave();
            CommonActions.LedeText().Should().Be("These are the results for this competition.".FormatForComparison());
        }

        public void ViewMultipleWinningResults()
        {
            CommonActions.ClickSave();
            CommonActions.InsetText().Should().Be("Information:Your competition has produced more than 1 solution with a winning score. You can therefore choose to procure any of the winning solutions.".FormatForComparison());
        }
    }
}
