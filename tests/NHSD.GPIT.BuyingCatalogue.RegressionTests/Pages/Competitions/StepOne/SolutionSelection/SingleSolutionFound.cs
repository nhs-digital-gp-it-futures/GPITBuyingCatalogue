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
            CommonActions.InsetText().Should().Be("Information: Your selected filter has only returned 1 result. You therefore do not need to carry out a competition and can instead use a direct award to procure this solution.".FormatForComparison());

            CommonActions.ClickFirstRadio();
            CommonActions.ClickSave();
        }
    }
}
