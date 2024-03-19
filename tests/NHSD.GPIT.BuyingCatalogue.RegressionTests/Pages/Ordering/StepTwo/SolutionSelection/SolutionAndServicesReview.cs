using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection
{
    public class SolutionAndServicesReview : PageBase
    {
        public SolutionAndServicesReview(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void ReviewSolutionAndServices()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            CommonActions.ClickContinue();
        }
    }
}
