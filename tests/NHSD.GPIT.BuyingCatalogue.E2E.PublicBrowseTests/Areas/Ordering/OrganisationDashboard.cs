using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrganisationDashboard
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly Dictionary<string, string> Parameters = new() { { "OdsCode", "03F" } };

        public OrganisationDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DashboardController),
                  nameof(DashboardController.Organisation),
                  Parameters)
        {
        }

        [Fact]
        public async Task OrganisationDashboard_OrganisationDashboard_SectionsCorrectlyDisplayed()
        {
            CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.CreateOrderLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Common.ByExtensions.DataTestId("incomplete-orders-table")).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Common.ByExtensions.DataTestId("complete-orders-table")).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var organisation = await context.Organisations.SingleAsync(o => o.OdsCode == Parameters["OdsCode"]);

            CommonActions.PageTitle().Should().BeEquivalentTo(organisation.Name.FormatForComparison());
        }

        [Fact]
        public void OrganisationDashboard_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                  typeof(HomeController),
                  nameof(HomeController.Index))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void OrganisationDashboard_CreateOrder_Click()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.OrganisationDashboard.CreateOrderLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.NewOrder)).Should().BeTrue();
        }
    }
}
