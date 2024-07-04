using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.AssociatedServiceOnly
{
    public class MergerAndSplit : PageBase
    {
        public MergerAndSplit(IWebDriver driver, CommonActions commonActions) : base(driver, commonActions)
        {
        }

        public void MergerAndSplitSolutionSelection()
        {
            CommonActions.HintText().Should().Be("There is only one solution with this type of Associated Service.".FormatForComparison());
            CommonActions.ClickSave();
        }
    }
}
