using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.Frameworks;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.Framework
{
    public class AddFramework : PageBase
    {
        public AddFramework(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void NewFramework()
        {
            CommonActions.ClickLinkElement(DashboardFrameworkObjects.AddNewFramwork);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FrameworksController),
                nameof(FrameworksController.Add))
                .Should().BeTrue();
        }

        public void AddFrameworkDetails()
        {
            TextGenerators.OrganisationInputAddText(AddFrameworkObjects.FrameworkNameInput, 30);
            CommonActions.ClickAllCheckboxes();
            CommonActions.ClickFirstRadio();
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FrameworksController),
                nameof(FrameworksController.Dashboard))
                .Should().BeTrue();

            CommonActions.ClickContinue();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index))
                .Should().BeTrue();
        }
    }
}
