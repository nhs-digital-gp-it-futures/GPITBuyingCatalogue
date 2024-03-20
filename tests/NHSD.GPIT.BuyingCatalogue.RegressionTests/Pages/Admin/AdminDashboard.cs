using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin
{
    public class AdminDashboard : PageBase
    {
        public AdminDashboard(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void ManageCapabilitiesAndEpics()
        {
            CommonActions.ClickLinkElement(HomeObjects.ManageCapabilitiesAndEpics);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(Gen2MappingController),
                nameof(Gen2MappingController.Capabilities))
                .Should().BeTrue();
        }
    }
}
