using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.EditSolution;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.HostingType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionApplicationType
{
    public class BrowserBased : PageBase
    {
        public BrowserBased(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
            SolutionApplicationTypes = new SolutionApplicationTypes(driver, commonActions);
        }

        internal SolutionApplicationTypes SolutionApplicationTypes { get; }

        public void AddBrowserBasedApplication()
        {
            CommonActions.ClickRadioButtonWithText("Browser-based");
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased))
                .Should().BeTrue();
        }

        public void AddBrowserBasedApplicationTypes()
        {
            var browserBasedApplication = GetBrowserBasedApplicationTypes();
            foreach (var type in browserBasedApplication)
            {
                switch (type)
                {
                    case nameof(BrowserBasedApplications.supported_browser):
                        AddSupportedBrowser(type);
                        break;
                    case nameof(BrowserBasedApplications.plug_ins_or_extensions):
                        AddPluginsOrExtensions(type);
                        break;
                    case nameof(BrowserBasedApplications.connectivity_and_resolution):
                        AddConnectivityResolution(type);
                        break;
                    case nameof(BrowserBasedApplications.hardware_requirements):
                        AddHardwareRequirements(type);
                        break;
                    default:
                        AddAdditionalInformation(type);
                        break;
                }
            }

            CommonActions.ClickContinue();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ApplicationType))
                .Should().BeTrue();
        }

        public void AddSupportedBrowser(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.SupportedBrowserLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.SupportedBrowsers))
                .Should().BeTrue();

            CommonActions.ClickAllCheckboxes();
            CommonActions.ClickFirstRadio();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased))
                .Should().BeTrue();
        }

        public void AddPluginsOrExtensions(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.PluginsOrExtensionsLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.PlugInsOrExtensions))
                .Should().BeTrue();

            CommonActions.ClickFirstRadio();
            TextGenerators.TextInputAddText(AddApplicationTypeObjects.AdditionalInformation, 500);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased))
                .Should().BeTrue();
        }

        public void AddConnectivityResolution(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.ConnectivityAndResolutionLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.ConnectivityAndResolution))
                .Should().BeTrue();

            CommonActions.SelectRandomDropDownItem(ApplicationTypeObjects.ConnectionSpeedDropdown);
            CommonActions.SelectRandomDropDownItem(ApplicationTypeObjects.ScreenResolutionDropdown);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased))
                .Should().BeTrue();
        }

        public void AddHardwareRequirements(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.HardwareRequirementsLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.HardwareRequirements))
                .Should().BeTrue();

            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Common.CommonSelectors.Description, 500);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased))
                .Should().BeTrue();
        }

        public void AddAdditionalInformation(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.AdditionalInformationLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.AdditionalInformation))
                .Should().BeTrue();

            TextGenerators.TextInputAddText(AddApplicationTypeObjects.AdditionalInformation, 500);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased))
                .Should().BeTrue();
        }

        public List<string> GetBrowserBasedApplicationTypes()
        {
            var browserBasedTypes = Enum.GetValues(typeof(BrowserBasedApplications))
            .Cast<BrowserBasedApplications>()
            .Select(v => v.ToString())
            .ToList();

            return browserBasedTypes;
        }
    }
}
