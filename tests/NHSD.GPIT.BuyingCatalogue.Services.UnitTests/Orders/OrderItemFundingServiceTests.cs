using System.Collections.Generic;
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
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public class OrderItemFundingServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderItemFundingService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_Solution_0Value_NoFundingRequired(
            OrderItem item,
            OrderItemFundingService service)
        {
            item.OrderItemPrice.OrderItemPriceTiers.ForEach(x => x.Price = 0);
            item.Quantity = 0;

            var result = await service.GetFundingType(item);

            result.Should().Be(OrderItemFundingType.NoFundingRequired);
        }

        [Theory]
        [CommonAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_AssociatedService_HasValue_NotForcedFunding(
            OrderItem item,
            OrderItemFundingService service)
        {
            item.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            var result = await service.GetFundingType(item);

            result.Should().Be(OrderItemFundingType.None);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_Solution_HasValue_LocalFundingOnly(
            OrderItem item,
            OrderItemFundingService service,
            [Frozen] BuyingCatalogueDbContext context)
        {
            context.Frameworks.Add(new EntityFramework.Catalogue.Models.Framework { Id = "LOCAL", LocalFundingOnly = true, Name = "local funding framework" });
            item.CatalogueItem.Solution = new Solution { CatalogueItemId = item.CatalogueItemId };
            item.CatalogueItem.Solution.FrameworkSolutions.Add(new FrameworkSolution { SolutionId = item.CatalogueItemId, FrameworkId = "LOCAL" });
            item.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            context.CatalogueItems.Add(item.CatalogueItem);

            await context.SaveChangesAsync();

            var result = await service.GetFundingType(item);

            result.Should().Be(OrderItemFundingType.LocalFundingOnly);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_Solution_HasValue_NotForcedFunding(
            OrderItem item,
            OrderItemFundingService service,
            [Frozen] BuyingCatalogueDbContext context)
        {
            var frameworkId = "NotForcedFunding";

            context.Frameworks.Add(new EntityFramework.Catalogue.Models.Framework { Id = frameworkId, LocalFundingOnly = false, Name = "NFF framework" });
            item.CatalogueItem.Solution = new Solution { CatalogueItemId = item.CatalogueItemId };
            item.CatalogueItem.Solution.FrameworkSolutions.Add(new FrameworkSolution { SolutionId = item.CatalogueItemId, FrameworkId = frameworkId });
            item.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            context.CatalogueItems.Add(item.CatalogueItem);

            await context.SaveChangesAsync();

            var result = await service.GetFundingType(item);

            result.Should().Be(OrderItemFundingType.None);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_AdditionalService_HasValue_LocalFundingOnly(
            OrderItem item,
            OrderItemFundingService service,
            [Frozen] BuyingCatalogueDbContext context)
        {
            var solutionId = new CatalogueItemId(1001, "001");
            var frameworkId = "LOCAL";

            context.Frameworks.Add(new EntityFramework.Catalogue.Models.Framework { Id = frameworkId, LocalFundingOnly = true, Name = "local funding framework" });

            var solution = new CatalogueItem
            {
                Id = solutionId,
                CatalogueItemType = CatalogueItemType.Solution,
                Name = "Solution",
                Solution = new Solution
                {
                    CatalogueItemId = solutionId,
                    FrameworkSolutions = new List<FrameworkSolution>
                    {
                        new()
                        {
                            SolutionId = solutionId,
                            FrameworkId = frameworkId,
                        },
                    },
                },
            };
            context.CatalogueItems.Add(solution);

            item.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;

            item.CatalogueItem.AdditionalService = new AdditionalService
            {
                CatalogueItemId = item.CatalogueItemId,
                SolutionId = solutionId,
            };

            context.CatalogueItems.Add(item.CatalogueItem);

            await context.SaveChangesAsync();

            var result = await service.GetFundingType(item);

            result.Should().Be(OrderItemFundingType.LocalFundingOnly);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_AdditionalService_HasValue_NotForcedFunding(
            OrderItem item,
            OrderItemFundingService service,
            [Frozen] BuyingCatalogueDbContext context)
        {
            var solutionId = new CatalogueItemId(1001, "001");
            var frameworkId = "NotForcedFunding";

            context.Frameworks.Add(new EntityFramework.Catalogue.Models.Framework { Id = frameworkId, LocalFundingOnly = false, Name = "NFF framework" });

            var solution = new CatalogueItem
            {
                Id = solutionId,
                CatalogueItemType = CatalogueItemType.Solution,
                Name = "Solution",
                Solution = new Solution
                {
                    CatalogueItemId = solutionId,
                    FrameworkSolutions = new List<FrameworkSolution>
                    {
                        new()
                        {
                            SolutionId = solutionId,
                            FrameworkId = frameworkId,
                        },
                    },
                },
            };
            context.CatalogueItems.Add(solution);

            item.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;

            item.CatalogueItem.AdditionalService = new AdditionalService
            {
                CatalogueItemId = item.CatalogueItemId,
                SolutionId = solutionId,
            };

            context.CatalogueItems.Add(item.CatalogueItem);

            await context.SaveChangesAsync();

            var result = await service.GetFundingType(item);

            result.Should().Be(OrderItemFundingType.None);
        }
    }
}
