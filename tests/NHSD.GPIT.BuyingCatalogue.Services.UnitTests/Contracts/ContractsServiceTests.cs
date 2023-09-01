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
            var contract = new Contract { OrderId = orderId };

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.GetContract(orderId);

            output.Should().BeEquivalentTo(contract);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContract_ContractDoesNotExist_ReturnsNewContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            var output = await service.GetContract(orderId);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(orderId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContractWithImplementationPlan_ContractExists_ReturnsContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service,
            ImplementationPlan implementationPlan)
        {
            var contract = new Contract { OrderId = orderId, ImplementationPlan = implementationPlan, };

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.GetContractWithImplementationPlan(orderId);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(orderId);
            output.ImplementationPlan.Should().NotBeNull();
            output.ImplementationPlan.Id.Should().Be(implementationPlan.Id);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContractWithImplementationPlan_ContractDoesNotExist_ReturnsNewContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            var output = await service.GetContractWithImplementationPlan(orderId);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(orderId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContractWithContractBilling_ContractExists_ReturnsContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service,
            ContractBilling contractBilling)
        {
            var contract = new Contract { OrderId = orderId, ContractBilling = contractBilling, };

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.GetContractWithContractBilling(orderId);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(orderId);
            output.ContractBilling.Should().NotBeNull();
            output.ContractBilling.Id.Should().Be(contractBilling.Id);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContractWithContractBilling_ContractDoesNotExist_ReturnsNewContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            var output = await service.GetContractWithContractBilling(orderId);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(orderId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContractWithRequirements_ContractExists_ReturnsContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service,
            ContractBilling contractBilling)
        {
            var contract = new Contract { OrderId = orderId, ContractBilling = contractBilling, };

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.GetContractWithContractBillingRequirements(orderId);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(orderId);
            output.ContractBilling.Should().NotBeNull();
            output.ContractBilling.Id.Should().Be(contractBilling.Id);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContractWithRequirements_ContractDoesNotExist_ReturnsNewContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            var output = await service.GetContractWithContractBillingRequirements(orderId);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(orderId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task RemoveContract_ContractExists_RemovesContract(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var contract = new Contract { OrderId = orderId };

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await service.RemoveContract(orderId);

            var actualContract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);
            actualContract.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task RemoveContract_ContractDoesNotExist_Returns(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            await service.RemoveContract(orderId);

            var actualContract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);
            actualContract.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task ResetContract_ContractExists_RemovesContractAndDataProcessing(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var contract = new Contract { OrderId = orderId };

            dbContext.Contracts.Add(contract);

            var contractFlag = new ContractFlags() { OrderId = orderId, UseDefaultDataProcessing = true };

            dbContext.ContractFlags.Add(contractFlag);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            await service.ResetContract(orderId);

            var actualContract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);
            actualContract.Should().BeNull();

            var actualFlags = await dbContext.ContractFlags.FirstAsync(x => x.OrderId == orderId);
            actualFlags.Should().NotBeNull();
            actualFlags.UseDefaultDataProcessing.Should().Be(false);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task ResetContract_ContractDoesNotExist_Returns(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            await service.ResetContract(orderId);

            var actualContract = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);
            actualContract.Should().BeNull();

            var actualFlags = await dbContext.ContractFlags.FirstAsync(x => x.OrderId == orderId);
            actualFlags.Should().NotBeNull();
            actualFlags.UseDefaultDataProcessing.Should().Be(false);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContractFlags_ContractFlagExists_ReturnsContractFlag(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var flags = new ContractFlags { OrderId = orderId };

            dbContext.ContractFlags.Add(flags);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.GetContractFlags(orderId);

            output.Should().BeEquivalentTo(flags);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContractFlags_ContractFlagDoesNotExist_CreatesNewContractFlag(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.ContractFlags.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            var output = await service.GetContractFlags(orderId);

            output.OrderId.Should().Be(orderId);

            var actual = await dbContext.ContractFlags.FirstOrDefaultAsync(x => x.OrderId == orderId);

            actual.Should().Be(output);
        }

        [Theory]
        [InMemoryDbInlineAutoData(true)]
        [InMemoryDbInlineAutoData(false)]
        public static async Task UseDefaultDataProcessing_UpdatesValue(
            bool value,
            int orderId,
            ContractFlags contractFlags,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            contractFlags.Order = null;
            contractFlags.OrderId = orderId;
            contractFlags.UseDefaultDataProcessing = null;

            dbContext.ContractFlags.Add(contractFlags);

            await dbContext.SaveChangesAsync();

            await service.UseDefaultDataProcessing(orderId, value);

            var actual = await dbContext.ContractFlags.FirstAsync(x => x.Id == contractFlags.Id);

            actual.UseDefaultDataProcessing.Should().Be(value);
        }
    }
}
