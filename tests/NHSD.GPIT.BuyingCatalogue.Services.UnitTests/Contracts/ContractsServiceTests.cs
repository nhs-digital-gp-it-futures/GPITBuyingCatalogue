﻿using System.Threading.Tasks;
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
            var contract = new Contract { OrderId = orderId };

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.GetContract(orderId);

            output.Should().BeEquivalentTo(contract);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContract_ContractDoesNotExist_ReturnsNull(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractsService service)
        {
            var existing = await dbContext.Contracts.FirstOrDefaultAsync(x => x.OrderId == orderId);

            existing.Should().BeNull();

            var output = await service.GetContract(orderId);

            output.Should().BeNull();
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
        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetContract_RemoveBillingAndRequirements(
            int orderId,
            [Frozen] BuyingCatalogueDbContext dbContext)
        {
            var flags = new ContractFlags
            {
                OrderId = orderId,
                UseDefaultBilling = true,
                HasSpecificRequirements = true,
            };
            dbContext.ContractFlags.Add(flags);

            await dbContext.SaveChangesAsync();
            ContractsService service = new ContractsService(dbContext);
            await service.RemoveBillingAndRequirements(orderId);
            var output = await service.GetContract(orderId);

            output.UseDefaultBilling.Should().BeNull();
            output.HasSpecificRequirements.Should().BeNull();
        }
    }
}
