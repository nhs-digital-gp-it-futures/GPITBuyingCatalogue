using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.ProxyBuyer
{
    [Collection(nameof(OrderingCollection))]
    public sealed class OrganisationDashboard : BuyerTestBase
    {
        private const string InternalOrgId = "CG-15F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrganisationDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DashboardController),
                  nameof(DashboardController.Organisation),
                  Parameters,
                  UserSeedData.AliceEmail)
        {
        }

        [Fact]
        public async Task OrganisationDashboard_SectionsCorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var organisation = await context.Organisations.FirstAsync(o => o.InternalIdentifier == Parameters["InternalOrgId"]);

            CommonActions.PageTitle().Should().BeEquivalentTo($"Orders dashboard - {organisation.Name}".FormatForComparison());
            CommonActions.LedeText().Should().BeEquivalentTo("Manage orders currently in progress and view completed orders.".FormatForComparison());
            CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.ActOnBehalf).Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.CreateOrderLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.SearchBar).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ByExtensions.DataTestId("orders-table")).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeFalse();
            CommonActions.ElementExists(CommonSelectors.PaginationPrevious).Should().BeFalse();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void OrganisationDashboard_CreateOrder_Click()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.OrganisationDashboard.CreateOrderLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.OrderItemType)).Should().BeTrue();
        }
    }
}
