using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionHostingType
{
    public class Hybrid : PageBase
    {
        public Hybrid(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddHostingTypeHybrid()
        {
            string hybrid = HostingTypes.Hybrid.ToString();
            CommonActions.ClickRadioButtonWithText(hybrid);
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HostingTypesController),
                nameof(HostingTypesController.Hybrid))
                .Should().BeTrue();

            AddHybridDetails();
        }

        public void AddHybridDetails()
        {
            TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_Summary, 500);
            TextGenerators.UrlInputAddText(HostingTypesObjects.HostingType_Link, 1000);
            TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_HostingModel, 1000);
            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HostingTypesController),
                nameof(HostingTypesController.HostingType))
                .Should().BeTrue();

            string hybridEditLink = HostingTypes.Hybrid.ToString();
            CommonActions.ElementIsDisplayed(HostingTypesObjects.HostingTypeEditLink(hybridEditLink));
        }
    }
}
