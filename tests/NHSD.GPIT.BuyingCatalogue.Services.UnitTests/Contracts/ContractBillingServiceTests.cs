using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Contracts;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;
using Contract = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Contract;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Contracts
{
    public class ContractBillingServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ContractBillingService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddContractBilling_ContractBillingExists_ReturnsExistingContract(
            Order order,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractBillingService service,
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

            var output = await service.AddContractBilling(order.Id, contract.Id);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(order.Id);
            output.ContractBilling.Should().NotBeNull();
            output.ContractBilling.Id.Should().Be(contractBilling.Id);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddContractBilling_ContractBillingDoesNotExist_ReturnsContract(
            Order order,
            [Frozen] BuyingCatalogueDbContext dbContext,
            Contract contract,
            ContractBillingService service)
        {
            dbContext.Orders.Add(order);

            contract.OrderId = order.Id;
            contract.Order = order;
            contract.ContractBilling = null;

            dbContext.Contracts.Add(contract);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.AddContractBilling(order.Id, contract.Id);

            output.Should().NotBeNull();
            output.OrderId.Should().Be(order.Id);
            output.ContractBilling.Should().NotBeNull();
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void AddBespokeContractBillingItem_NullOrEmptyName_ThrowsException(
            string name,
            int orderId,
            int contractId,
            CatalogueItemId catalogueItemId,
            string paymentTrigger,
            int quantity,
            ContractBillingService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddBespokeContractBillingItem(
                        orderId,
                        contractId,
                        catalogueItemId,
                        name,
                        paymentTrigger,
                        quantity))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(name));
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void AddBespokeContractBillingItem_NullOrEmptyPaymentTrigger_ThrowsException(
            string paymentTrigger,
            int orderId,
            int contractId,
            CatalogueItemId catalogueItemId,
            string name,
            int quantity,
            ContractBillingService service)
        {
            FluentActions
                .Awaiting(
                    () => service.AddBespokeContractBillingItem(
                        orderId,
                        contractId,
                        catalogueItemId,
                        name,
                        paymentTrigger,
                        quantity))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(paymentTrigger));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddBespokeContractBillingItem_NullContractBilling_ContractBillingAndItemCreated(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItemId catalogueItemId,
            string paymentTrigger,
            string name,
            int quantity,
            Contract contract,
            Order order,
            ContractBillingService service)
        {
            order.OrderItems.Add(new OrderItem()
            {
                CatalogueItemId = catalogueItemId,
                CatalogueItem = new CatalogueItem() { Name = "Test", Id = catalogueItemId, CatalogueItemType = CatalogueItemType.AssociatedService, },
            });
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            contract.Order = order;
            contract.ContractBilling = null;
            context.Contracts.Add(contract);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.AddBespokeContractBillingItem(
                order.Id,
                contract.Id,
                catalogueItemId,
                name,
                paymentTrigger,
                quantity);

            var actual = await context.Contracts.FirstAsync(f => f.Id == contract.Id);
            actual.ContractBilling.Should().NotBeNull();
            actual.ContractBilling.ContractBillingItems.Should().NotBeNull();
            actual.ContractBilling.ContractBillingItems.Count.Should().Be(1);

            var newMilestone = actual.ContractBilling.ContractBillingItems.First();
            newMilestone.CatalogueItemId.Should().Be(catalogueItemId);
            newMilestone.Milestone.Title.Should().Be(name);
            newMilestone.Milestone.PaymentTrigger.Should().Be(paymentTrigger);
            newMilestone.Quantity.Should().Be(quantity);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddBespokeContractBillingItem_ExistingContractBilling_ItemAdded(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItemId catalogueItemId,
            string paymentTrigger,
            string name,
            int quantity,
            Order order,
            Contract contract,
            ContractBillingService service)
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

            await service.AddBespokeContractBillingItem(
                order.Id,
                contract.Id,
                catalogueItemId,
                name,
                paymentTrigger,
                quantity);

            var actual = await context.Contracts.FirstAsync(f => f.Id == contract.Id);
            actual.ContractBilling.Should().NotBeNull();
            actual.ContractBilling.ContractBillingItems.Should().NotBeNull();
            actual.ContractBilling.ContractBillingItems.Count.Should().Be(1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetContractBillingItem_NoMilestoneExists_ReturnsNull(
            int orderId,
            int itemId,
            ContractBillingService service)
        {
            var item = await service.GetContractBillingItem(orderId, itemId);

            item.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetContractBillingItem_ItemExists_ReturnsItem(
            [Frozen] BuyingCatalogueDbContext dbContext,
            ContractBillingItem contractBillingItem,
            Order order,
            OrderItem orderItem,
            ContractBillingService service)
        {
            dbContext.Orders.Add(order);

            orderItem.Order = order;
            dbContext.OrderItems.Add(orderItem);

            contractBillingItem.OrderId = order.Id;
            contractBillingItem.OrderItem = orderItem;

            dbContext.ContractBillingItems.Add(contractBillingItem);

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var output = await service.GetContractBillingItem(order.Id, contractBillingItem.Id);

            output.Id.Should().Be(contractBillingItem.Id);
            output.CatalogueItemId.Should().Be(contractBillingItem.CatalogueItemId);
            output.Milestone.Title.Should().Be(contractBillingItem.Milestone.Title);
            output.Milestone.PaymentTrigger.Should().Be(contractBillingItem.Milestone.PaymentTrigger);
            output.Quantity.Should().Be(contractBillingItem.Quantity);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void EditContractBillingItem_NullOrEmptyName_ThrowsException(
           string name,
           int orderId,
           int itemId,
           CatalogueItemId catalogueItemId,
           string paymentTrigger,
           int quantity,
           ContractBillingService service)
        {
            FluentActions
                .Awaiting(
                    () => service.EditContractBillingItem(
                        orderId,
                        itemId,
                        catalogueItemId,
                        name,
                        paymentTrigger,
                        quantity))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(name));
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void EditContractBillingItem_NullOrEmptyPaymentTrigger_ThrowsException(
            string paymentTrigger,
            int orderId,
            int itemId,
            CatalogueItemId catalogueItemId,
            string name,
            int quantity,
            ContractBillingService service)
        {
            FluentActions
                .Awaiting(
                    () => service.EditContractBillingItem(
                        orderId,
                        itemId,
                        catalogueItemId,
                        name,
                        paymentTrigger,
                        quantity))
                .Should()
                .ThrowAsync<ArgumentNullException>(nameof(paymentTrigger));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task EditMilestone_ExistingMilestone_MilestoneUpdated(
            [Frozen] BuyingCatalogueDbContext context,
            string paymentTrigger,
            CatalogueItemId catalogueItemId,
            string name,
            int quantity,
            OrderItem orderItem,
            ContractBillingItem item,
            Order order,
            ContractBillingService service)
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

            context.ContractBillingItems.Add(item);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var before = await context.ContractBillingItems.Include(x => x.Milestone).FirstAsync(f => f.Id == item.Id);
            before.CatalogueItemId.Should().Be(item.CatalogueItemId);
            before.Milestone.Title.Should().Be(item.Milestone.Title);
            before.Milestone.PaymentTrigger.Should().Be(item.Milestone.PaymentTrigger);
            before.Quantity.Should().Be(item.Quantity);

            await service.EditContractBillingItem(
                order.Id,
                item.Id,
                catalogueItemId,
                name,
                paymentTrigger,
                quantity);

            var after = await context.ContractBillingItems.Include(x => x.Milestone).FirstAsync(f => f.Id == item.Id);
            after.CatalogueItemId.Should().Be(catalogueItemId);
            after.Milestone.Title.Should().Be(name);
            after.Milestone.PaymentTrigger.Should().Be(paymentTrigger);
            after.Quantity.Should().Be(quantity);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DeleteContractBillingItem_ExistingMilestone_MilestoneDeleted(
            [Frozen] BuyingCatalogueDbContext context,
            OrderItem orderItem,
            ContractBillingItem item,
            Order order,
            ContractBillingService service)
        {
            context.Orders.Add(order);

            orderItem.Order = order;
            context.OrderItems.Add(orderItem);

            item.OrderId = order.Id;
            item.OrderItem = orderItem;

            context.ContractBillingItems.Add(item);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var before = await context.ContractBillingItems.FirstOrDefaultAsync(f => f.Id == item.Id);
            before.Should().NotBeNull();

            await service.DeleteContractBillingItem(
                order.Id,
                item.Id);

            var actual = await context.ContractBillingItems.FirstOrDefaultAsync(f => f.Id == item.Id);
            actual.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DeleteContractBillingItems_ExistingMilestone_MilestoneDeleted(
            [Frozen] BuyingCatalogueDbContext context,
            OrderItem orderItem,
            CatalogueItemId catalogueItemId,
            ContractBillingItem item,
            Order order,
            ContractBillingService service)
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

            context.ContractBillingItems.Add(item);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var before = await context.ContractBillingItems.FirstOrDefaultAsync(f => f.CatalogueItemId == catalogueItemId);
            before.Should().NotBeNull();

            await service.DeleteContractBillingItems(
                order.Id,
                new List<CatalogueItemId>() { catalogueItemId, });

            var actual = await context.ContractBillingItems.FirstOrDefaultAsync(f => f.CatalogueItemId == catalogueItemId);
            actual.Should().BeNull();
        }
    }
}
