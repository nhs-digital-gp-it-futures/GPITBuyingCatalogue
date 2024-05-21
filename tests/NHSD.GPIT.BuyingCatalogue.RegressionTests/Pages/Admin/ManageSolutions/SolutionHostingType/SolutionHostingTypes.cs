using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionHostingType
{
    public class SolutionHostingTypes : PageBase
    {
        public SolutionHostingTypes(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddHostingType(string solutionId)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.SolutionHostingTypeLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HostingTypesController),
                nameof(HostingTypesController.HostingType))
                .Should().BeTrue();
        }

        public void HostingTypeDashboard()
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.AddHostingTypeLink);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HostingTypesController),
                nameof(HostingTypesController.AddHostingType))
                .Should().BeTrue();
        }

        public void CatalogueSolutionDashboard()
        {
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should().BeTrue();
        }
    }
}
