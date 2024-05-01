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
    public class MobileOrTablet : PageBase
    {
        public MobileOrTablet(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
            SolutionApplicationTypes = new SolutionApplicationTypes(driver, commonActions);
        }

        internal SolutionApplicationTypes SolutionApplicationTypes { get; }

        public void AddMobileOrTabletApplication()
        {
            CommonActions.ClickRadioButtonWithText("Mobile or tablet");
            CommonActions.ClickSave();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MobileTablet))
                .Should().BeTrue();
        }

        public void AddMobileOrTabletApplicationTypes()
        {
            var mobilOrTableApplication = GetMobileOrTabletApplicationTypes();
            foreach (var type in mobilOrTableApplication)
            {
                switch (type)
                {
                    case nameof(MobileOrTabletApplications.operating_systems):
                        AddSupportedOperatingSystams(type);
                        break;
                    case nameof(MobileOrTabletApplications.connectivity):
                        AddConnectivity(type);
                        break;
                    case nameof(MobileOrTabletApplications.memory_and_storage):
                        AddMemoryAndStorage(type);
                        break;
                }
            }
        }

        public void AddSupportedOperatingSystams(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.SupportedOperatingSystemLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.OperatingSystems))
                .Should().BeTrue();

            CommonActions.ClickAllCheckboxes();
            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Common.CommonSelectors.Description, 500);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MobileTablet))
                .Should().BeTrue();
        }

        public void AddConnectivity(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.ConnectivityLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.Connectivity))
                .Should().BeTrue();

            CommonActions.ClickAllCheckboxes();
            CommonActions.SelectRandomDropDownItem(ApplicationTypeObjects.ConnectionSpeedDropdown);
            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Common.CommonSelectors.Description, 500);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MobileTablet))
                .Should().BeTrue();
        }

        public void AddMemoryAndStorage(string type)
        {
            var value = SolutionApplicationTypes.GetApplicationTypeValue(type);
            CommonActions.ClickLinkElement(AddApplicationTypeObjects.MemoryAndStorageLink(value));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MemoryAndStorage))
                .Should().BeTrue();

            CommonActions.SelectRandomDropDownItem(ApplicationTypeObjects.MinimumMemoryDropDown);
            TextGenerators.TextInputAddText(E2ETests.Framework.Objects.Common.CommonSelectors.Description, 500);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(MobileTabletBasedController),
                nameof(MobileTabletBasedController.MobileTablet))
                .Should().BeTrue();
        }

        public List<string> GetMobileOrTabletApplicationTypes()
        {
            var browserBasedTypes = Enum.GetValues(typeof(MobileOrTabletApplications))
            .Cast<MobileOrTabletApplications>()
            .Select(v => v.ToString())
            .ToList();

            return browserBasedTypes;
        }
    }
}
