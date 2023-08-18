using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne.SolutionSelection
{
    public class NoSolutionsFound : PageBase
    {
        public NoSolutionsFound(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void NoSolutions()
        {
            CommonActions.LedeText().Should().Be("There were no results from your chosen filter.".FormatForComparison());
            CommonActions.ClickStartNewSearch();
        }
    }
}
