using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition
{
    public class StartCompetition : PageBase
    {
        public StartCompetition(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CreateCompetition(string competition)
        {
            CommonActions.LedeText().Should().Be("Provide a name and description for your competition.".FormatForComparison());

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(CompetitionsDashboardController),
               nameof(CompetitionsDashboardController.SaveCompetition))
           .Should()
           .BeTrue();

            Driver.FindElement(CreateCompetitionObjects.CompetitionName).SendKeys(GetDescription(competition));

            TextGenerators.TextInputAddText(CreateCompetitionObjects.CompetitionDescription, 100);

            CommonActions.ClickSave();
        }

        public string GetDescription(string competition)
        {
            var rnd = new Random();

            var newDescription = competition + rnd.Next(100).ToString();
            return newDescription;
        }
    }
}
