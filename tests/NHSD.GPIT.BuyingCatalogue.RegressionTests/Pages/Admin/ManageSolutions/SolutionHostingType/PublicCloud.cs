using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionHostingType
{
    public class PublicCloud : PageBase
    {
        public PublicCloud(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddHostingTypePublicCloud()
        {
            string publicCloud = HostingTypes.Public_cloud.ToString().Replace("_", " ");
            CommonActions.ClickRadioButtonWithText(publicCloud);
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HostingTypesController),
                nameof(HostingTypesController.PublicCloud))
                .Should().BeTrue();

            AddPublicCloudDetails();
        }

        public void AddPublicCloudDetails()
        {
            TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_Summary, 500);
            TextGenerators.UrlInputAddText(HostingTypesObjects.HostingType_Link, 500);
            CommonActions.ClickFirstCheckbox();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HostingTypesController),
                nameof(HostingTypesController.HostingType))
                .Should().BeTrue();

            string publicCloudLink = HostingTypes.Public_cloud.ToString().Replace("_", "-");
            CommonActions.ElementIsDisplayed(HostingTypesObjects.PublicCloudEditLink(publicCloudLink));
        }
    }
}
