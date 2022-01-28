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
        public void Dashboard_AllSectionsDisplayed()
        {
            CommonActions.WaitUntilElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.InactiveItemsContainer);

            CommonActions.PageTitle().Should().Be("Supplier defined Epics".FormatForComparison());
            CommonActions.LedeText().Should().Be("Add a supplier defined Epic or edit an existing one.".FormatForComparison());

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.InactiveItemsContainer)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.EpicsTable)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.AddEpicLink)
                .Should()
                .BeTrue();

            CommonActions.ElementIsNotDisplayed(SupplierDefinedEpicsDashboardObjects.InactiveItemRow)
                .Should()
                .BeTrue();
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

        private static string GetActiveStatusText(bool isActive) => isActive ? "Active" : "Inactive";
    }
}
