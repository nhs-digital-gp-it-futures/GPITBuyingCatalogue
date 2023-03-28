using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    [Collection(nameof(OrderingCollection))]
    public sealed class NewOrderDashboard
        : BuyerTestBase
    {
        private const string InternalOrgId = "CG-03F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public NewOrderDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrderController),
                  nameof(OrderController.NewOrder),
                  Parameters)
        {
        }

        [Fact]
        public async Task NewOrderDashboard_AllSectionsDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var organisation = await context.Organisations.FirstAsync(o => o.InternalIdentifier == Parameters["InternalOrgId"]);

            CommonActions.PageTitle().Should().BeEquivalentTo($"New Order-{organisation.Name}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.TaskList)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.OrderDescriptionLink)
                .Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrderDashboard.OrderDescriptionStatus)
                .Should().BeTrue();

            CommonActions.ElementExists(Objects.Ordering.OrderDashboard.LastUpdatedEndNote)
                .Should().BeFalse();
        }

        [Fact]
        public void NewOrderDashboard_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrderController),
                    nameof(OrderController.ReadyToStart))
                    .Should().BeTrue();
        }

        [Fact]
        public void NewOrderDashboard_ClickOrderDescription()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.OrderDashboard.OrderDescriptionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderDescriptionController),
                nameof(OrderDescriptionController.NewOrderDescription))
                    .Should().BeTrue();
        }
    }
}
