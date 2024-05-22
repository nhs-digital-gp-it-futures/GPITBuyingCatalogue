using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SolutionSelection
{
    public class SelectSolutions : PageBase
    {
        public SelectSolutions(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddSolutions(int numberofSolutions)
        {
            //CommonActions.HintText().Should().Be("These are the results from your chosen filter. You must provide a reason if any of the solutions listed are not taken through to your competition shortlist.".FormatForComparison());

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(CompetitionSelectSolutionsController),
               nameof(CompetitionSelectSolutionsController.SelectSolutions))
           .Should()
           .BeTrue();

            CommonActions.ClickMultipleCheckboxes(numberofSolutions);
            CommonActions.ClickSave();
        }
    }
}
