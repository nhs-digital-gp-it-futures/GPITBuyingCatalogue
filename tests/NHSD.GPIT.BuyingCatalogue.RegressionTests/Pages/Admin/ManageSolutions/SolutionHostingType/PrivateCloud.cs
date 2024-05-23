using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionHostingType
{
    public class PrivateCloud : PageBase
    {
        public PrivateCloud(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddHostingTypePrivateCloud()
        {
            string privateCloud = HostingTypes.Private_cloud.ToString().Replace("_", " ");
            CommonActions.ClickRadioButtonWithText(privateCloud);
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HostingTypesController),
                nameof(HostingTypesController.PrivateCloud))
                .Should().BeTrue();

            AddPrivateCloudDetails();
        }

        public void AddPrivateCloudDetails()
        {
            TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_Summary, 500);
            TextGenerators.UrlInputAddText(HostingTypesObjects.HostingType_Link, 500);
            TextGenerators.TextInputAddText(HostingTypesObjects.HostingType_HostingModel, 1000);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HostingTypesController),
                nameof(HostingTypesController.HostingType))
                .Should().BeTrue();

            string privateCloudEditLink = HostingTypes.Private_cloud.ToString().Replace("_", "-");
            CommonActions.ElementIsDisplayed(HostingTypesObjects.HostingTypeEditLink(privateCloudEditLink));
        }
    }
}
