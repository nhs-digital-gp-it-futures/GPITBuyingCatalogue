using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.SupplierDefinedEpics
{
    [Collection(nameof(AdminCollection))]
    public sealed class Dashboard : AuthorityTestBase
    {
        public Dashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SupplierDefinedEpicsController),
                  nameof(SupplierDefinedEpicsController.Dashboard),
                  null)
        {
        }

        [Fact]
        public void Dashboard_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.PageTitle().Should().Be("Supplier defined Epics".FormatForComparison());
            CommonActions.LedeText().Should().Be("Add a supplier defined Epic or edit an existing one.".FormatForComparison());

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.EpicsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.AddEpicLink).Should().BeTrue();
        }

        [Fact]
        public void Dashboard_ClickHomeBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Dashboard_EpicsTable_ContainsEditLinkForEachEpic()
        {
            using var context = GetEndToEndDbContext();
            var epics = context.Epics.Where(e => e.SupplierDefined).ToList();

            epics.ForEach(epic =>
                Driver
                    .FindElement(By.XPath($"//tr[td//text()[contains(., '{epic.Name}')]]/td[4]/a"))
                    .GetAttribute("href")
                    .Should()
                    .Contain(epic.Id));
        }

        [Fact]
        public void Dashboard_ClickAddOrganisationLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(SupplierDefinedEpicsDashboardObjects.AddEpicLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.SelectCapabilities)).Should().BeTrue();
        }

        private PageSummary GetPageSummary() => new()
        {
            Names = Driver.FindElements(SupplierDefinedEpicsDashboardObjects.EpicNames).Select(s => s.GetAttribute("data-name").Trim()),
            Capabilities = Driver.FindElements(SupplierDefinedEpicsDashboardObjects.CapabilityNames).Select(s => s.GetAttribute("data-capability").Trim()),
            Ids = Driver.FindElements(SupplierDefinedEpicsDashboardObjects.EpicIds).Select(s => s.GetAttribute("data-id").Trim()),
        };

        private readonly struct PageSummary
        {
            public IEnumerable<string> Names { get; init; }

            public IEnumerable<string> Capabilities { get; init; }

            public IEnumerable<string> Ids { get; init; }
        }
    }
}
