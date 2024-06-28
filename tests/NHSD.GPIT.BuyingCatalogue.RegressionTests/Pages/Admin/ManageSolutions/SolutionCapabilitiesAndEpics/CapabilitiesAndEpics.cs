using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionCapabilitiesAndEpics
{
    public class CapabilitiesAndEpics : PageBase
    {
        public CapabilitiesAndEpics(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
            : base(driver, commonActions)
        {
            Factory = factory;
        }

        public LocalWebApplicationFactory Factory { get; }

        public void AddCapabilitiesAndEpics(string solutionId)
        {
            CommonActions.ClickLinkElement(AddSolutionObjects.CapabilitiesAndEpicsLink(solutionId));
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditCapabilities))
                .Should().BeTrue();

            AddCapabilitiesAndEpicsDetails();
        }

        public void AddCapabilitiesAndEpicsDetails()
        {
            using var dbContext = Factory.DbContext;
            var capabilities = dbContext.Capabilities.Include(c => c.Epics).Where(x => x.Status == CapabilityStatus.Effective).ToList().Take(5);

            foreach (var capability in capabilities)
                CommonActions.ClickCheckboxByLabel($"({capability.CapabilityRef}) {capability.Name}");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();
        }
    }
}
