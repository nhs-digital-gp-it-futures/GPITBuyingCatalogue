using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SelectAdditionalServiceRecipients : PageBase
    {
        public SelectAdditionalServiceRecipients(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddServiceRecipients()
        {
            CommonActions.PageLoadedCorrectGetIndex(
             typeof(ServiceRecipientsController),
             nameof(ServiceRecipientsController.AddServiceRecipients)).Should().BeTrue();

            CommonActions.ClickSave();
        }
    }
}
