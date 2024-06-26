using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Shortlist;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionApplicationType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Shortlist
{
    public class ShortlistDashboard : PageBase
    {
        private const int MaxNumberOfShortlists = 10;

        public ShortlistDashboard(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void ShortlistTriage()
        {
            CommonActions.ClickLinkElement(OrganisationDashboard.ViewShorlist);
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.Index)).Should().BeTrue();

            CommonActions.ClickLinkElement(ShortlistObjects.CreateNewShortlist);
        }

        public void CreateShortlist()
        {
                CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index)).Should().BeTrue();
        }

        public void MaximumShortlists()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.MaximumShortlists)).Should().BeTrue();
        }

        public void FilterByFoundationCapabilities()
        {
            CommonActions.ClickLinkElement(ShortlistObjects.FilterByFoundationCapabilitiesLink);

            CommonActions.ClickSaveFilters();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.ConfirmSaveFilter)).Should().BeTrue();
        }

        public void FilterByFramework()
        {
            CommonActions.ClickLinkElement(ShortlistObjects.FilterByFoundationCapabilitiesLink);
            CommonActions.ClickRadioButtonWithText("Tech Innovation");

            CommonActions.ClickSaveFilters();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.ConfirmSaveFilter)).Should().BeTrue();
        }

        public void FilterByApplicationType(ApplicationTypes applicationType)
        {
            CommonActions.ClickLinkElement(ShortlistObjects.FilterByFoundationCapabilitiesLink);
            CommonActions.ClickCheckboxByLabel(applicationType.ToString());
            CommonActions.ClickSaveFilters();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.ConfirmSaveFilter)).Should().BeTrue();
        }

        public void SaveFilter(string shortlistName)
        {
            var randomText = TextGenerators.TextInput(3);
            var newShortlistName = $"{shortlistName} {randomText}";

            Driver.FindElement(ShortlistObjects.FitlerName).SendKeys(newShortlistName);
            TextGenerators.TextInputAddText(ShortlistObjects.FilterDescription, 50);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.FilterDetails)).Should().BeTrue();
        }
    }
}
