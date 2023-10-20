using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo
{
    public class CalculatePrice : PageBase
    {
        public CalculatePrice(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CompetitionCalculatePrice(string solutionId)
        {
            CommonActions.ClickLinkElement(CreateCompetitionObjects.CalculatePriceEditLink, solutionId);
            CommonActions.LedeText().Should().Be("Provide information to calculate the price for this shortlisted solution and any Additional or Associated Services you’ll need.".FormatForComparison());
        }
    }
}
