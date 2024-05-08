using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderFrameworkServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderFrameworkService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
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
        [MockInMemoryDbAutoData]
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
            order.AssociatedServicesOnlyDetails.Solution = solution.CatalogueItem;

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
