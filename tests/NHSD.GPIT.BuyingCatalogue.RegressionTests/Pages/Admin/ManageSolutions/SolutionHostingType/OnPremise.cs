using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionHostingType
{
    public class OnPremise : PageBase
    {
        public OnPremise(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void AddHostingTypeOnPremise()
        {
            string onPremise = HostingTypes.On_premise.ToString().Replace("_", " ");
            CommonActions.ClickRadioButtonWithText(onPremise);
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HostingTypesController),
                nameof(HostingTypesController.OnPremise))
                .Should().BeTrue();

            AddOnPremiseDetails();
        }

        public void AddOnPremiseDetails()
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

            string onPremiseEditLink = HostingTypes.Private_cloud.ToString().Replace("_", "-");
            CommonActions.ElementIsDisplayed(HostingTypesObjects.HostingTypeEditLink(onPremiseEditLink));
        }
    }
}
