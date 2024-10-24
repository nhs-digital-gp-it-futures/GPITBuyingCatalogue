﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.Gen2
{
    public class CapabilitiesAndEpicsMappings : PageBase
    {
        public CapabilitiesAndEpicsMappings(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void ImportCapabilities(string fileName)
        {
            var importFile = CommonActions.ImportCsvFile(fileName);

            CommonActions.UploadFile(ImportObjects.ImportCsvFileInput, importFile);

            CommonActions.ClickSave();
        }

        public void ImportEpics(string fileName)
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(Gen2MappingController),
                nameof(Gen2MappingController.Epics))
                .Should().BeTrue();

            var importFile = CommonActions.ImportCsvFile(fileName);
            CommonActions.UploadFile(ImportObjects.ImportCsvFileInput, importFile);
            CommonActions.ClickSave();
        }

        public void CapabilitiesAndEpicsMappingSuccess()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(Gen2MappingController),
                nameof(Gen2MappingController.Mapping))
                .Should().BeTrue();

            CommonActions.ClickReturnToAdminHomePage();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index))
                .Should().BeTrue();
        }

        public void CapabilitiesAndEpicsMappinFailed()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(Gen2MappingController),
                nameof(Gen2MappingController.FailedCapabilities))
                .Should().BeTrue();

            CommonActions.ClickReturnToAdminHomePage();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index))
                .Should().BeTrue();
        }

        public void SolutionsAndEpicsMappinFailed()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(Gen2MappingController),
                nameof(Gen2MappingController.FailedEpics))
                .Should().BeTrue();

            CommonActions.ClickReturnToAdminHomePage();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index))
                .Should().BeTrue();
        }
    }
}
