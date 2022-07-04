using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using LinqKit;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Framework;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Frameworks
{
    public class FrameworkServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FrameworkService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFramework_NoOrderFound_ReturnsNull(FrameworkService service)
        {
            var result = await service.GetFramework(0);

            result.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFramework_NoSolutionId_ReturnsNull(
            Order order,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync();

            var result = await service.GetFramework(order.Id);

            result.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFramework_NoSolutionId_AssociatedServicesOnly_ReturnsNull(
            Order order,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            order.AssociatedServicesOnly = true;
            order.SolutionId = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync();

            var result = await service.GetFramework(order.Id);

            result.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFramework_SolutionExists_CovidFrameworkOnly_ReturnsNull(
            Order order,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            dbContext.Orders.Add(order);
            dbContext.FrameworkSolutions.Add(new FrameworkSolution
            {
                SolutionId = order.OrderItems.First().CatalogueItemId,
                FrameworkId = FrameworkService.CovidFrameworkId,
            });

            await dbContext.SaveChangesAsync();

            var result = await service.GetFramework(order.Id);

            result.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFramework_SolutionExists_CovidFrameworkOnly_AssociatedServicesOnly_ReturnsNull(
            Order order,
            CatalogueItemId solutionId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            order.AssociatedServicesOnly = true;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.SolutionId = solutionId;

            dbContext.Orders.Add(order);
            dbContext.FrameworkSolutions.Add(new FrameworkSolution
            {
                SolutionId = solutionId,
                FrameworkId = FrameworkService.CovidFrameworkId,
            });

            await dbContext.SaveChangesAsync();

            var result = await service.GetFramework(order.Id);

            result.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFramework_SolutionExists_ReturnsExpectedResult(
            Order order,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            dbContext.Orders.Add(order);
            dbContext.Frameworks.Add(framework);
            dbContext.FrameworkSolutions.Add(new FrameworkSolution
            {
                SolutionId = order.OrderItems.First().CatalogueItemId,
                FrameworkId = framework.Id,
            });

            await dbContext.SaveChangesAsync();

            var result = await service.GetFramework(order.Id);

            result.Should().BeEquivalentTo(framework);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetFramework_SolutionExists_AssociatedServicesOnly_ReturnsExpectedResult(
            Order order,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] BuyingCatalogueDbContext dbContext,
            FrameworkService service)
        {
            order.AssociatedServicesOnly = true;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.SolutionId = order.Solution.Id;

            dbContext.Orders.Add(order);
            dbContext.Frameworks.Add(framework);
            dbContext.FrameworkSolutions.Add(new FrameworkSolution
            {
                SolutionId = order.SolutionId.Value,
                FrameworkId = framework.Id,
            });

            await dbContext.SaveChangesAsync();

            var result = await service.GetFramework(order.Id);

            result.Should().BeEquivalentTo(framework);
        }
    }
}
