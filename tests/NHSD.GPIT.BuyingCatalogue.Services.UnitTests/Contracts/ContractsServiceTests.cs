using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Contracts;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Contracts
{
    public class ContractsServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ContractsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContract_ContractExists_ReturnsContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            dbContext.Contracts.Add(new Contract { OrderId = orderId });

            await dbContext.SaveChangesAsync();

            var output = await service.GetContract(orderId);

            output.OrderId.Should().Be(orderId);
            output.BillingItems.Should().BeEmpty();
            output.DataProcessingPlan.Should().BeNull();
            output.ImplementationPlan.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContract_ContractDoesNotExist_CreatesNewContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            var output = await service.GetContract(orderId);

            output.OrderId.Should().Be(orderId);
            output.BillingItems.Should().BeEmpty();
            output.DataProcessingPlan.Should().BeNull();
            output.ImplementationPlan.Should().BeNull();

            var actual = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            actual.Should().NotBeNull();
            actual!.OrderId.Should().Be(orderId);
            actual.DataProcessingPlanId.Should().BeNull();
            actual.ImplementationPlanId.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetImplementationPlanId_ContractDoesNotExist_NoActionTaken(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            await service.SetImplementationPlanId(orderId, null);

            var actual = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetImplementationPlanId_ContractExists_ExpectedResult(
            int orderId,
            int implementationPlanId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            dbContext.Contracts.Add(new Contract { OrderId = orderId });

            await dbContext.SaveChangesAsync();

            await service.SetImplementationPlanId(orderId, implementationPlanId);

            var actual = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            actual.Should().NotBeNull();
            actual!.ImplementationPlanId.Should().Be(implementationPlanId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetDataProcessingPlanId_ContractDoesNotExist_NoActionTaken(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            await service.SetDataProcessingPlanId(orderId, null);

            var actual = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetDataProcessingPlanId_ContractExists_ExpectedResult(
            int orderId,
            int dataProcessingPlanId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            dbContext.Contracts.Add(new Contract { OrderId = orderId });

            await dbContext.SaveChangesAsync();

            await service.SetDataProcessingPlanId(orderId, dataProcessingPlanId);

            var actual = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            actual.Should().NotBeNull();
            actual!.DataProcessingPlanId.Should().Be(dataProcessingPlanId);
        }
    }
}
