using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo
{
    public class SolutionServicePrice : PageBase
    {
        public SolutionServicePrice(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void SolutionConfirmPrice(string solutionId)
        {
            CommonActions.ClickLinkElement(PriceAndQuantityObjects.SolutionPriceEditLink, solutionId);
        }

    }
}
