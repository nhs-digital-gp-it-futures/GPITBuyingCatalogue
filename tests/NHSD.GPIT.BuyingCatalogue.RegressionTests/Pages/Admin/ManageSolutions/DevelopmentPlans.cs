using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions
{
    public class DevelopmentPlans : PageBase
    {
        public DevelopmentPlans(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddWorkOffPlan(string solutionId)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.DevelopmentPlansLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.DevelopmentPlans))
                .Should().BeTrue();

            WorkOffPlanDetails();
            ManageDevelopmentPlans();
        }

        private void WorkOffPlanDetails()
        {
            CommonActions.ClickLinkElement(DevelopmentPlanObjects.WorkOffPlansActionLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.AddWorkOffPlan))
                .Should().BeTrue();

            CommonActions.SelectDropDownItem(DevelopmentPlanObjects.SelectStandards, 1);
            TextGenerators.TextInputAddText(DevelopmentPlanObjects.Details, 300);
            TextGenerators.DateInputAddDateSoon(
                                E2ETests.Framework.Objects.Common.CommonSelectors.DateDay,
                                E2ETests.Framework.Objects.Common.CommonSelectors.DateMonth,
                                E2ETests.Framework.Objects.Common.CommonSelectors.DateYear);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DevelopmentPlansController),
                nameof(DevelopmentPlansController.DevelopmentPlans))
                .Should()
                .BeTrue();
        }

        private void ManageDevelopmentPlans()
        {
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }
    }
}
