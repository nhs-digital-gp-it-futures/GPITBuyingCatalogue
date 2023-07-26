using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Numerics;
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
using Contract = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Contract;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Contracts
{
    public class RequirementsServiceTest
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(RequirementsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetRequirementComplete_ContractBillingExists_SetsFlagTrue(
            Order order,
            [Frozen] BuyingCatalogueDbContext dbContext,
            RequirementsService service,
            ContractBilling contractBilling,
            Contract contract)
        {
            dbContext.Orders.Add(order);

            contract.OrderId = order.Id;
            contract.Order = order;
            contract.ContractBilling = contractBilling;

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.SetRequirementComplete(order.Id, contract.Id);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(order.Id);
            output.ContractBilling.Should().NotBeNull();
            output.ContractBilling.Id.Should().Be(contractBilling.Id);
            output.ContractBilling.HasConfirmedRequirements.Should().BeTrue();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetRequirementComplete_ContractBillingDoesNotExist_SetsFlagTrue(
            Order order,
            [Frozen] BuyingCatalogueDbContext dbContext,
            Contract contract,
            RequirementsService service)
        {
            dbContext.Orders.Add(order);

            contract.OrderId = order.Id;
            contract.Order = order;
            contract.ContractBilling = null;

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.SetRequirementComplete(order.Id, contract.Id);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(order.Id);
            output.ContractBilling.Should().NotBeNull();
            output.ContractBilling.HasConfirmedRequirements.Should().BeTrue();
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void AddRequirement_NullOrEmptyDetails_ThrowsException(
            string details,
            int orderId,
            int contractId,
            CatalogueItemId catalogueItemId,
            RequirementsService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddRequirement(
                        orderId,
                        contractId,
                        catalogueItemId,
                        details))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(details));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddRequirement_ExistingContractBilling_ItemAdded(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItemId catalogueItemId,
            string details,
            Order order,
            Contract contract,
            RequirementsService service)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = catalogueItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = catalogueItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            contract.Order = order;
            contract.ContractBilling = new ContractBilling();

            context.Contracts.Add(contract);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.AddRequirement(
                order.Id,
                contract.Id,
                catalogueItemId,
                details);

            var actual = await context.Contracts.FirstAsync(f => f.Id == contract.Id);
            actual.ContractBilling.Should().NotBeNull();
            actual.ContractBilling.Requirements.Should().NotBeNull();
            actual.ContractBilling.Requirements.Count.Should().Be(1);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetRequirement_NoRequirementExists_ReturnsNull(
            int orderId,
            int itemId,
            RequirementsService service)
        {
            var item = await service.GetRequirement(orderId, itemId);

            item.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetRequirement_ItemExists_ReturnsItem(
            [Frozen] BuyingCatalogueDbContext dbContext,
            Requirement requirement,
            Order order,
            OrderItem orderItem,
            RequirementsService service)
        {
            dbContext.Orders.Add(order);

            orderItem.Order = order;
            dbContext.OrderItems.Add(orderItem);

            requirement.OrderId = order.Id;
            requirement.OrderItem = orderItem;

            dbContext.Requirements.Add(requirement);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.GetRequirement(order.Id, requirement.Id);

            output.Id.Should().Be(requirement.Id);
            output.CatalogueItemId.Should().Be(requirement.CatalogueItemId);
            output.Details.Should().Be(requirement.Details);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void EditRequirement_NullOrEmptyDetails_ThrowsException(
           string details,
           int orderId,
           int itemId,
           CatalogueItemId catalogueItemId,
           RequirementsService service)
        {
            FluentActions
                .Awaiting(
                    () => service.EditRequirement(
                        orderId,
                        itemId,
                        catalogueItemId,
                        details))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(details));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EditRequirement_ExistingRequirement_RequirementUpdated(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItemId catalogueItemId,
            string details,
            OrderItem orderItem,
            Requirement item,
            Order order,
            RequirementsService service)
        {
            context.Orders.Add(order);

            orderItem.Order = order;
            orderItem.CatalogueItemId = catalogueItemId;
            orderItem.CatalogueItem = new CatalogueItem()
            {
                Name = "Test", Id = catalogueItemId, CatalogueItemType = CatalogueItemType.AssociatedService,
            };

            context.OrderItems.Add(orderItem);

            item.OrderId = order.Id;
            item.OrderItem = orderItem;

            context.Requirements.Add(item);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var before = await context.Requirements.FirstAsync(f => f.Id == item.Id);
            before.CatalogueItemId.Should().Be(item.CatalogueItemId);
            before.Details.Should().Be(item.Details);

            await service.EditRequirement(
                order.Id,
                item.Id,
                catalogueItemId,
                details);

            var after = await context.Requirements.FirstAsync(f => f.Id == item.Id);
            after.CatalogueItemId.Should().Be(catalogueItemId);
            after.Details.Should().Be(details);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteRequirement_ExistingRequirement_RequirementDeleted(
            [Frozen] BuyingCatalogueDbContext context,
            OrderItem orderItem,
            Requirement item,
            Order order,
            RequirementsService service)
        {
            context.Orders.Add(order);

            orderItem.Order = order;
            context.OrderItems.Add(orderItem);

            item.OrderId = order.Id;
            item.OrderItem = orderItem;

            context.Requirements.Add(item);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var before = await context.Requirements.FirstOrDefaultAsync(f => f.Id == item.Id);
            before.Should().NotBeNull();

            await service.DeleteRequirement(
                order.Id,
                item.Id);

            var actual = await context.Requirements.FirstOrDefaultAsync(f => f.Id == item.Id);
            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteRequirements_ExistingRequirement_RequirementDeleted(
            [Frozen] BuyingCatalogueDbContext context,
            OrderItem orderItem,
            CatalogueItemId catalogueItemId,
            Requirement item,
            Order order,
            RequirementsService service)
        {
            context.Orders.Add(order);

            orderItem.Order = order;
            orderItem.CatalogueItem = new CatalogueItem()
            {
                Name = "Test", CatalogueItemType = CatalogueItemType.AssociatedService, Id = catalogueItemId,
            };
            orderItem.CatalogueItemId = catalogueItemId;
            context.OrderItems.Add(orderItem);

            item.OrderId = order.Id;
            item.OrderItem = orderItem;
            item.CatalogueItemId = catalogueItemId;

            context.Requirements.Add(item);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var before = await context.Requirements.FirstOrDefaultAsync(f => f.CatalogueItemId == catalogueItemId);
            before.Should().NotBeNull();

            await service.DeleteRequirements(
                order.Id,
                new List<CatalogueItemId>() { catalogueItemId, });

            var actual = await context.Requirements.FirstOrDefaultAsync(f => f.CatalogueItemId == catalogueItemId);
            actual.Should().BeNull();
        }
    }
}
