using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderFrameworkServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderFrameworkService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbInlineAutoData(FundingType.Local)]
        [InMemoryDbInlineAutoData(FundingType.GPIT)]
        public static async Task UpdateFundingSourceAndSetSelectedFrameworkForOrder_NotGP_LocalFrameworkOnlyUnchanged_UpdatesSelectedFramework(
            FundingType fundingType,
            Order order,
            OrderItem orderItem,
            EntityFramework.Catalogue.Models.Framework selectedFramework,
            [Frozen] BuyingCatalogueDbContext context,
            OrderFrameworkService service)
        {
            var fundingTypes = new List<FundingType> { fundingType };

            selectedFramework.FundingTypes = fundingTypes;

            orderItem.OrderItemFunding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly;

            order.OrderItems.Clear();
            order.OrderItems.Add(orderItem);
            order.OrderingParty.OrganisationType = OrganisationType.IB;
            order.SelectedFramework.FundingTypes = fundingTypes;

            context.Frameworks.Add(selectedFramework);
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.UpdateFundingSourceAndSetSelectedFrameworkForOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, selectedFramework.Id);

            var result = await context.Orders.FirstAsync(x => x.Id == order.Id);
            result.SelectedFramework.Should().BeEquivalentTo(selectedFramework);
            result.OrderItems.Count.Should().Be(order.OrderItems.Count);
            result.OrderItems.First().OrderItemFunding.Should().BeEquivalentTo(orderItem.OrderItemFunding);
        }

        [Theory]
        [InMemoryDbInlineAutoData(FundingType.Local, FundingType.GPIT)]
        [InMemoryDbInlineAutoData(FundingType.GPIT, FundingType.Local)]
        public static async Task UpdateFundingSourceAndSetSelectedFrameworkForOrder_GPPractice_LocalFrameworkOnlyChanged_UpdatesSelectedFramework(
            FundingType fundingType,
            FundingType orderFundingType,
            Order order,
            OrderItem orderItem,
            EntityFramework.Catalogue.Models.Framework selectedFramework,
            [Frozen] BuyingCatalogueDbContext context,
            OrderFrameworkService service)
        {
            var fundingTypes = new List<FundingType> { fundingType };

            selectedFramework.FundingTypes = fundingTypes;

            orderItem.OrderItemFunding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly;

            order.OrderItems.Clear();
            order.OrderItems.Add(orderItem);
            order.OrderingParty.OrganisationType = OrganisationType.GP;
            order.SelectedFramework.FundingTypes = new List<FundingType> { orderFundingType };

            context.Frameworks.Add(selectedFramework);
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.UpdateFundingSourceAndSetSelectedFrameworkForOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, selectedFramework.Id);

            var result = await context.Orders.FirstAsync(x => x.Id == order.Id);
            result.SelectedFramework.Should().BeEquivalentTo(selectedFramework);
            result.OrderItems.Count.Should().Be(order.OrderItems.Count);
            result.OrderItems.First().OrderItemFunding.Should().BeEquivalentTo(orderItem.OrderItemFunding);
        }

        [Theory]
        [InMemoryDbInlineAutoData(FundingType.Local, FundingType.GPIT)]
        [InMemoryDbInlineAutoData(FundingType.GPIT, FundingType.Local)]
        public static async Task UpdateFundingSourceAndSetSelectedFrameworkForOrder_NotGP_LocalFrameworkOnlyChanged_OrderItemFundingNull(
            FundingType fundingType,
            FundingType orderFundingType,
            Order order,
            OrderItem orderItem,
            EntityFramework.Catalogue.Models.Framework selectedFramework,
            [Frozen] BuyingCatalogueDbContext context,
            OrderFrameworkService service)
        {
            var fundingTypes = new List<FundingType> { fundingType };

            selectedFramework.FundingTypes = fundingTypes;

            orderItem.OrderItemFunding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly;

            order.OrderItems.Clear();
            order.OrderItems.Add(orderItem);
            order.OrderingParty.OrganisationType = OrganisationType.IB;
            order.SelectedFramework.FundingTypes = new List<FundingType> { orderFundingType };

            context.Frameworks.Add(selectedFramework);
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.UpdateFundingSourceAndSetSelectedFrameworkForOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, selectedFramework.Id);

            var result = await context.Orders.FirstAsync(x => x.Id == order.Id);
            result.SelectedFramework.Should().BeEquivalentTo(selectedFramework);
            result.OrderItems.Count.Should().Be(order.OrderItems.Count);
            result.OrderItems.First().OrderItemFunding.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFrameworksForOrder_Solution_ReturnsExpected(
            Order order,
            OrderItem orderItem,
            Solution solution,
            List<FrameworkSolution> frameworkSolutions,
            [Frozen] BuyingCatalogueDbContext context,
            OrderFrameworkService service)
        {
            var frameworks = frameworkSolutions.Select(x => x.Framework).Distinct().ToList();
            frameworks.Take(1).ToList().ForEach(x => x.IsExpired = true);
            frameworks.Skip(1).ToList().ForEach(x => x.IsExpired = false);
            frameworkSolutions.ForEach(
                x =>
                {
                    x.SolutionId = default;
                    x.Solution = solution;
                });

            order.OrderItems = new List<OrderItem> { orderItem };

            orderItem.CatalogueItem = solution.CatalogueItem;
            solution.FrameworkSolutions = frameworkSolutions;

            context.Frameworks.AddRange(frameworks);
            context.Solutions.Add(solution);
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var result = await service.GetFrameworksForOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, false);

            result.Should().BeEquivalentTo(frameworks.Skip(1));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFrameworksForOrder_AssociatedService_ReturnsExpected(
            Order order,
            OrderItem orderItem,
            AssociatedService associatedService,
            Solution solution,
            List<FrameworkSolution> frameworkSolutions,
            [Frozen] BuyingCatalogueDbContext context,
            OrderFrameworkService service)
        {
            var frameworks = frameworkSolutions.Select(x => x.Framework).Distinct().ToList();
            frameworks.Take(1).ToList().ForEach(x => x.IsExpired = true);
            frameworks.Skip(1).ToList().ForEach(x => x.IsExpired = false);
            frameworkSolutions.ForEach(
                x =>
                {
                    x.SolutionId = default;
                    x.Solution = solution;
                });

            order.OrderItems = new List<OrderItem> { orderItem };
            order.Solution = solution.CatalogueItem;

            orderItem.CatalogueItem = associatedService.CatalogueItem;
            solution.FrameworkSolutions = frameworkSolutions;

            context.Frameworks.AddRange(frameworks);
            context.Solutions.Add(solution);
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var result = await service.GetFrameworksForOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, true);

            result.Should().BeEquivalentTo(frameworks.Skip(1));
        }
    }
}
