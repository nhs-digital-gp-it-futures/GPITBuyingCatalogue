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
    public class Desktop : PageBase
    {
        public Desktop(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
            SolutionApplicationTypes = new SolutionApplicationTypes(driver, commonActions);
        }

        internal SolutionApplicationTypes SolutionApplicationTypes { get; }

        public void AddDesktopApplication()
        {
            string desktop = ApplicationTypes.Desktop.ToString();
            CommonActions.ClickRadioButtonWithText(desktop);
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();
        }

        public void AdDesktopApplicationTypes()
        {
            var desktopBasedTypes = GetDesktopApplicationTypes();
            foreach (var type in desktopBasedTypes)
            {
                switch (type)
                {
                    case nameof(DesktopApplications.operating_systems):
                        AddDesktopOperatingSystem(type);
                        break;
                    case nameof(DesktopApplications.connectivity):
                        AddConnectivity(type);
                        break;
                    case nameof(DesktopApplications.memory_and_storage):
                        AddMemoryAndStorage(type);
                        break;
                    case nameof(DesktopApplications.third_party_components):
                        AddThirdPartyComponents(type);
                        break;
                    case nameof(DesktopApplications.hardware_requirements):
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

        public void AddDesktopOperatingSystem(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.DesktopOperatingSystemLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.OperatingSystems))
                .Should().BeTrue();

            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Common.CommonSelectors.Description, 500);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();
        }

        public void AddConnectivity(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.DesktopConnectivityLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Connectivity))
                .Should().BeTrue();

            CommonActions.SelectRandomDropDownItem(ApplicationTypeObjects.ConnectionSpeedDropdown);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();
        }

        public void AddMemoryAndStorage(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.DesktopMemoryAndStorageLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.MemoryAndStorage))
                .Should().BeTrue();

            CommonActions.SelectRandomDropDownItem(E2ETests.Framework.Objects.Admin.EditSolution.ApplicationTypeObjects.MinimumMemoryDropDown);
            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Admin.EditSolution.ApplicationTypeObjects.StorageSpace, 300);
            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Admin.EditSolution.ApplicationTypeObjects.ProcessingPower, 300);
            CommonActions.SelectRandomDropDownItem(E2ETests.Framework.Objects.Admin.EditSolution.ApplicationTypeObjects.ResolutionDropdown);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();
        }

        public void AddThirdPartyComponents(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.DesktopThirdPartyComponentsLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.ThirdPartyComponents))
                .Should().BeTrue();

            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Admin.EditSolution.ApplicationTypeObjects.ThirdPartyComponents, 500);
            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Admin.EditSolution.ApplicationTypeObjects.DeviceCapabilities, 500);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();
        }

        public void AddHardwareRequirements(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.DesktopHardwareRequirementsLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.HardwareRequirements))
                .Should().BeTrue();

            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Common.CommonSelectors.Description, 500);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();
        }

        public void AddAdditionalInformation(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.DesktopAdditionalInformationLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.AdditionalInformation))
                .Should().BeTrue();

            TextGenerators.TextInputAddText(AddApplicationTypeObjects.AdditionalInformation, 500);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DesktopBasedController),
                nameof(DesktopBasedController.Desktop))
                .Should().BeTrue();
        }

        public List<string> GetDesktopApplicationTypes()
        {
            var desktopBasedTypes = Enum.GetValues(typeof(DesktopApplications))
            .Cast<DesktopApplications>()
            .Select(v => v.ToString())
            .ToList();

            return desktopBasedTypes;
        }
    }
}
