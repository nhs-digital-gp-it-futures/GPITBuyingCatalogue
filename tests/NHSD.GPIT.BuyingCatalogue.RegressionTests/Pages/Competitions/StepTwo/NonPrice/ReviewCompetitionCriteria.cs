using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice
{
    public class ReviewCompetitionCriteria : PageBase
    {
        public ReviewCompetitionCriteria(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void ReviewCriteria()
        {
            CommonActions.ClickSave();
            CommonActions.HintText().Should().Be("Complete the following steps to carry out a competition.".FormatForComparison());
        }
    }
}
