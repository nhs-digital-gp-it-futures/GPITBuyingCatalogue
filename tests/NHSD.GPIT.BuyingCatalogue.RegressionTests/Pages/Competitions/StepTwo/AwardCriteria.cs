using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo
{
    public class AwardCriteria : PageBase
    {
        public AwardCriteria(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void PriceONly()
        {
            CommonActions.ClickRadioButtonWithText("Price only");
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Complete the following steps to carry out a competition.".FormatForComparison());
        }

        public void PriceAndNonPrice()
        {
            CommonActions.ClickRadioButtonWithText("Price and non-price elements");
            CommonActions.ClickSave();

            CommonActions.LedeText().Should().Be("Complete the following steps to carry out a competition.".FormatForComparison());
        }
    }
}
