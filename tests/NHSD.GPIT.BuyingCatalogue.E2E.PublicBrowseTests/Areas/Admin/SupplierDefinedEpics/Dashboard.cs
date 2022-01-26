using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.SupplierDefinedEpics
{
    public sealed class Dashboard : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
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
        public void Index_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be("Supplier defined Epics".FormatForComparison());
            CommonActions.LedeText().Should().Be("Add a supplier defined Epic or edit an existing one.".FormatForComparison());

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.EpicsTable)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.AddEpicLink)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Index_EpicsTable_ContainsEditLinkForEachEpic()
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
        public void Index_EpicsTable_DisplaysStatusesCorrectly()
        {
            using var context = GetEndToEndDbContext();
            var epics = context.Epics.Where(e => e.SupplierDefined).ToList();

            epics.ForEach(epic =>
                Driver
                    .FindElement(By.XPath($"//tr[td//text()[contains(., '{epic.Name}')]]/td[3]/strong"))
                    .Text
                    .EqualsIgnoreWhiteSpace(GetActiveStatusText(epic.IsActive))
                    .Should()
                    .BeTrue());
        }

        private static string GetActiveStatusText(bool isActive) => isActive ? "Active" : "Inactive";
    }
}
