using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
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
        }

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
               if (type == BrowserBasedApplication.supported_browser.ToString())
               {
                    AddSupportedBrowser(type);
               }
            }
        }

        public void AddSupportedBrowser(string type)
        {
            var value = GetApplicationTypeValue(type);
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

        public List<string> GetBrowserBasedApplicationTypes()
        {
            var browserBasedTypes = Enum.GetValues(typeof(BrowserBasedApplication))
            .Cast<BrowserBasedApplication>()
            .Select(v => v.ToString())
            .ToList();

            return browserBasedTypes;
        }

        private string GetApplicationTypeValue(string type)
        {
            var value = type.Replace("_", "-");
            return value;
        }
    }
}
