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
            CommonActions.WaitUntilElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.InactiveItemsContainer);

            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.PageTitle().Should().Be("Supplier defined Epics".FormatForComparison());
            CommonActions.LedeText().Should().Be("Add a supplier defined Epic or edit an existing one.".FormatForComparison());

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.InactiveItemsContainer).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.EpicsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.AddEpicLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchBar).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchButton).Should().BeTrue();
            CommonActions.ElementIsNotDisplayed(SupplierDefinedEpicsDashboardObjects.InactiveItemRow).Should().BeTrue();
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
        public void Dashboard_ClickShowInactiveEpics_ExpectedResult()
        {
            CommonActions.ElementIsNotDisplayed(SupplierDefinedEpicsDashboardObjects.InactiveItemRow).Should().BeTrue();

            CommonActions.ClickFirstCheckbox();

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.InactiveItemRow).Should().BeTrue();
        }

        [Fact]
        public void Dashboard_ClickAddOrganisationLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(SupplierDefinedEpicsDashboardObjects.AddEpicLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.SelectCapabilities)).Should().BeTrue();
        }

        [Fact]
        public async Task Dashboard_SearchTermEmpty_AllEpicsDisplayed()
        {
            await using var context = GetEndToEndDbContext();

            var epics = context.Epics
                .Include(x => x.Capabilities)
                .Where(x => x.SupplierDefined);

            CommonActions.ElementAddValue(SupplierDefinedEpicsDashboardObjects.SearchBar, string.Empty);
            CommonActions.ClickLinkElement(SupplierDefinedEpicsDashboardObjects.SearchButton);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SupplierDefinedEpicsController),
                    nameof(SupplierDefinedEpicsController.Dashboard))
                .Should()
                .BeTrue();

            var pageSummary = GetPageSummary();

            pageSummary.Names.Should().BeEquivalentTo(epics.Select(x => x.Name.Trim()));
            pageSummary.Capabilities.Should().BeEquivalentTo(epics.Select(x => x.Capabilities.First().Name.Trim()));
            pageSummary.Ids.Should().BeEquivalentTo(epics.Select(x => x.Id.Trim()));
        }

        [Fact]
        public async Task Dashboard_SearchTermValid_FilteredEpicsDisplayed()
        {
            await using var context = GetEndToEndDbContext();

            var sampleEpic = context.Epics
                .Include(x => x.Capabilities)
                .Where(x => x.SupplierDefined)
                .OrderByDescending(x => x.Name.Length)
                .First();

            await CommonActions.InputCharactersWithDelay(
                SupplierDefinedEpicsDashboardObjects.SearchBar,
                sampleEpic.Name);
            CommonActions.WaitUntilElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchListBox);

            CommonActions.ElementExists(SupplierDefinedEpicsDashboardObjects.SearchResult(0)).Should().BeTrue();
            CommonActions.ElementTextEqualTo(
                    SupplierDefinedEpicsDashboardObjects.SearchResultTitle(0),
                    sampleEpic.Name)
                .Should()
                .BeTrue();

            CommonActions.ElementTextEqualTo(
                    SupplierDefinedEpicsDashboardObjects.SearchResultDescription(0),
                    sampleEpic.Capabilities.First().Name)
                .Should()
                .BeTrue();

            CommonActions.ClickLinkElement(SupplierDefinedEpicsDashboardObjects.SearchButton);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SupplierDefinedEpicsController),
                    nameof(SupplierDefinedEpicsController.Dashboard))
                .Should()
                .BeTrue();

            var pageSummary = GetPageSummary();

            pageSummary.Names.First().Should().Be(sampleEpic.Name.Trim());
            pageSummary.Capabilities.First().Should().Be(sampleEpic.Capabilities.First().Name.Trim());
            pageSummary.Ids.First().Should().Be(sampleEpic.Id);
        }

        [Fact]
        public async Task Dashboard_SearchTermValid_NoMatches_ErrorMessageDisplayed()
        {
            await CommonActions.InputCharactersWithDelay(
                SupplierDefinedEpicsDashboardObjects.SearchBar,
                Strings.RandomString(10));
            CommonActions.WaitUntilElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchListBox);

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchResultsErrorMessage)
                .Should()
                .BeTrue();

            CommonActions.ClickLinkElement(SupplierDefinedEpicsDashboardObjects.SearchButton);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SupplierDefinedEpicsController),
                    nameof(SupplierDefinedEpicsController.Dashboard))
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.AddEpicLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchBar).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.EpicsTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchErrorMessage).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchErrorMessageLink)
                .Should()
                .BeTrue();

            var pageSummary = GetPageSummary();

            pageSummary.Names.Should().BeEmpty();
            pageSummary.Capabilities.Should().BeEmpty();
            pageSummary.Ids.Should().BeEmpty();

            CommonActions.ClickLinkElement(SupplierDefinedEpicsDashboardObjects.SearchErrorMessageLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SupplierDefinedEpicsController),
                    nameof(SupplierDefinedEpicsController.Dashboard))
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.AddEpicLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchBar).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.EpicsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchErrorMessage)
                .Should()
                .BeFalse();
            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.SearchErrorMessageLink)
                .Should()
                .BeFalse();
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
