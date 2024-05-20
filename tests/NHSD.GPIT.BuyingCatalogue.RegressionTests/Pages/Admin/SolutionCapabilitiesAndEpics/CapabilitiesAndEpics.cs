using CsvHelper;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.SolutionCapabilitiesAndEpics
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
            var capability = dbContext.Capabilities.Include(c => c.Epics).Where(x => x.Status == CapabilityStatus.Effective).FirstOrDefault();
            //var capability1 = dbContext.Capabilities.Include(c => c.Epics)
            //    .Where(x => x.Status == CapabilityStatus.Effective)
            //    .Select(y => y.CapabilityEpics).FirstOrDefault();

            var mustEpic = capability.CapabilityEpics.First(e => e.CompliancyLevel == CompliancyLevel.Must).Epic;

            //string capability = capability.Result.CapabilityEpics.First().ToString();

            //var mustEpic = capability.CapabilityEpics.First(e => e.CompliancyLevel == CompliancyLevel.Must).Epic;

            CommonActions.ClickCheckboxByLabel($"({capability.CapabilityRef}) {capability.Name}");
            //CommonActions.ClickCheckboxByLabel($"({mustEpic.Id}) {mustEpic.Name}");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution))
                .Should()
                .BeTrue();
        }
    }
}
