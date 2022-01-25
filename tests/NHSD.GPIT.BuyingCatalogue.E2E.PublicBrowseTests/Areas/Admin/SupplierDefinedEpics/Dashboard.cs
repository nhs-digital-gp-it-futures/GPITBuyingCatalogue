using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
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

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.EpicsTable)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(SupplierDefinedEpicsDashboardObjects.AddEpicLink)
                .Should()
                .BeTrue();
        }
    }
}
