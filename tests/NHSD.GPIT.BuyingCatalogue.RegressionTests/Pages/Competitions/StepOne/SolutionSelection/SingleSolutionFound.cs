using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne.SolutionSelection
{
    public class SingleSolutionFound : PageBase
    {
        public SingleSolutionFound(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SingleSolution()
        {
            CommonActions.LedeText().Should().Be("These are the results from your chosen filter.".FormatForComparison());

            CommonActions.ClickFirstRadio();
            CommonActions.ClickSave();
        }
    }
}
