﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Notify.Client;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    [SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "Skipping tests that use Temporal queries")]
    public static class OrderServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrderWithCatalogueItemAndPrices_ReturnsExpectedResults(
           Order order,
           OrderItem orderItem,
           CatalogueItem catalogueItem,
           Organisation organisation,
           EntityFramework.Catalogue.Models.Framework selectedFramework,
           [Frozen] BuyingCatalogueDbContext context,
           OrderService service)
        {
            order.OrderingPartyId = organisation.Id;
            order.OrderingParty = organisation;

            order.SelectedFrameworkId = selectedFramework.Id;
            order.SelectedFramework = selectedFramework;

            orderItem.CatalogueItem = catalogueItem;
            order.OrderRecipients.ForEach(r => r.OrderItemRecipients.Clear());
            order.OrderItems.Clear();
            order.OrderItems.Add(orderItem);

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = (await service.GetOrderWithCatalogueItemAndPrices(order.CallOffId, order.OrderingParty.InternalIdentifier)).Order;

            result.OrderingParty.Should().BeEquivalentTo(organisation);
            result.SelectedFramework.Should().BeEquivalentTo(selectedFramework);
            result.OrderItems.Count.Should().Be(1);
            var actual = result.OrderItems.First();
            actual.CatalogueItem.Id.Should().Be(orderItem.CatalogueItem.Id);
            actual.CatalogueItem.CataloguePrices.Count.Should().Be(orderItem.CatalogueItem.CataloguePrices.Count);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrderWithOrderItems_ReturnsExpectedResults(
            Order order,
            OrderItem orderItem,
            CatalogueItem catalogueItem,
            Organisation organisation,
            EntityFramework.Catalogue.Models.Framework selectedFramework,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.OrderingPartyId = organisation.Id;
            order.OrderingParty = organisation;

            order.SelectedFrameworkId = selectedFramework.Id;
            order.SelectedFramework = selectedFramework;

            orderItem.CatalogueItem = catalogueItem;
            order.OrderRecipients.ForEach(r => r.OrderItemRecipients.Clear());
            order.OrderItems.Clear();
            order.OrderItems.Add(orderItem);

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = (await service.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier)).Order;

            result.OrderingParty.Should().BeEquivalentTo(organisation);
            result.SelectedFramework.Should().BeEquivalentTo(selectedFramework);
            result.OrderItems.Count.Should().Be(1);
            var actual = result.OrderItems.First();
            actual.CatalogueItem.Id.Should().Be(orderItem.CatalogueItem.Id);
            actual.CatalogueItem.CataloguePrices.Count.Should().Be(0);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrderWithOrderItemsForFunding_ReturnsExpectedResults(
            Order order,
            OrderItem orderItem,
            CatalogueItem catalogueItem,
            Organisation organisation,
            EntityFramework.Catalogue.Models.Framework selectedFramework,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.OrderingPartyId = organisation.Id;
            order.OrderingParty = organisation;

            order.SelectedFrameworkId = selectedFramework.Id;
            order.SelectedFramework = selectedFramework;

            orderItem.CatalogueItem = catalogueItem;
            order.OrderRecipients.ForEach(r => r.OrderItemRecipients.Clear());
            order.OrderItems.Clear();
            order.OrderItems.Add(orderItem);

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = (await service.GetOrderWithOrderItemsForFunding(order.CallOffId, order.OrderingParty.InternalIdentifier)).Order;

            result.OrderingParty.Should().BeEquivalentTo(organisation);
            result.SelectedFramework.Should().BeEquivalentTo(selectedFramework);
            result.OrderItems.Count.Should().Be(1);
            var actual = result.OrderItems.First();
            actual.CatalogueItem.Id.Should().Be(orderItem.CatalogueItem.Id);
            actual.OrderItemFunding.Should().NotBeNull();
            actual.OrderItemFunding.OrderItemFundingType.Should().Be(orderItem.OrderItemFunding.OrderItemFundingType);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrderWithSupplier_ReturnsExpectedResults(
            Order order,
            Supplier supplier,
            Contact supplierContact,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.Supplier = supplier;
            order.SupplierContact = supplierContact;

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = (await service.GetOrderWithSupplier(order.CallOffId, order.OrderingParty.InternalIdentifier)).Order;

            result.CallOffId.Should().Be(order.CallOffId);
            result.Supplier.Should().NotBeNull();
            result.SupplierContact.Should().NotBeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrderForTaskListStatuses_ReturnsExpectedResults(
            Order order,
            Supplier supplier,
            Contact supplierContact,
            Contact orderingPartyContact,
            ContractFlags contractFlags,
            Contract contract,
            ImplementationPlan implementationPlan,
            ContractBilling contractBilling,
            Organisation orderingParty,
            List<OrderItem> orderItems,
            EntityFramework.Catalogue.Models.Framework framework,
            List<OrderRecipient> orderRecipients,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            contract.ImplementationPlan = implementationPlan;
            contract.ContractBilling = contractBilling;

            order.Supplier = supplier;
            order.SupplierContact = supplierContact;
            order.SupplierContact = supplierContact;
            order.ContractFlags = contractFlags;
            order.Contract = contract;
            order.OrderItems = orderItems;
            order.OrderingPartyContact = orderingPartyContact;
            order.OrderingParty = orderingParty;
            order.SelectedFramework = framework;
            order.OrderRecipients = orderRecipients;

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var result = (await service.GetOrderForTaskListStatuses(order.CallOffId, order.OrderingParty.InternalIdentifier)).Order;

            result.CallOffId.Should().Be(order.CallOffId);
            result.Supplier.Should().NotBeNull();
            result.SupplierContact.Should().NotBeNull();
            result.ContractFlags.Should().NotBeNull();
            result.Contract.Should().NotBeNull();
            result.Contract.ImplementationPlan.Should().NotBeNull();
            result.Contract.ContractBilling.Should().NotBeNull();
            result.OrderItems.Count.Should().BeGreaterThan(0);
            result.OrderItems.ForEach(i => i.CatalogueItem.Should().NotBeNull());
            result.OrderItems.ForEach(i => i.OrderItemFunding.Should().NotBeNull());
            result.OrderItems.ForEach(i => i.OrderItemPrice.Should().NotBeNull());
            result.OrderingPartyContact.Should().NotBeNull();
            result.OrderingParty.Should().NotBeNull();
            result.SelectedFramework.Should().NotBeNull();
            result.OrderRecipients.Count.Should().BeGreaterThan(0);
            result.OrderRecipients.ForEach(i => i.OrderItemRecipients.Should().NotBeNull());
            result.OrderRecipients.ForEach(i => i.OdsOrganisation.Should().NotBeNull());
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(null)]
        [MockInMemoryDbInlineAutoData("")]
        [MockInMemoryDbInlineAutoData("   ")]
        public static async Task CreateOrder_InvalidFrameworkArgument_Throws(
            string frameworkId,
            [Frozen] BuyingCatalogueDbContext context,
            string description,
            Organisation organisation,
            OrderService service)
        {
            await context.Organisations.AddAsync(organisation);
            await context.SaveChangesAsync();

            await FluentActions.Invoking(async () => await service.CreateOrder(description, organisation.InternalIdentifier, OrderTypeEnum.Unknown, frameworkId))
                .Should()
                .ThrowAsync<ArgumentException>();
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.Unknown, "frameworkIdDoesNotExist")]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.Solution, "frameworkIdDoesNotExist")]
        public static async Task CreateOrder_OrderType_FrameworkId_Throws(
            OrderTypeEnum orderType,
            string frameworkId,
            [Frozen] BuyingCatalogueDbContext context,
            string description,
            Organisation organisation,
            OrderService service)
        {
            await context.Organisations.AddAsync(organisation);
            await context.SaveChangesAsync();

            await FluentActions.Invoking(async () => await service.CreateOrder(description, organisation.InternalIdentifier, orderType, frameworkId))
                .Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task CreateOrder_OrderType_Unknown_Throws(
            [Frozen] BuyingCatalogueDbContext context,
            string description,
            Organisation organisation,
            EntityFramework.Catalogue.Models.Framework framework,
            OrderService service)
        {
            framework.IsExpired = true;

            await context.Organisations.AddAsync(organisation);
            await context.Frameworks.AddAsync(framework);
            await context.SaveChangesAsync();

            await FluentActions.Invoking(async () => await service.CreateOrder(description, organisation.InternalIdentifier, OrderTypeEnum.Unknown, framework.Id))
                .Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task CreateOrder_Framework_Unknown_Throws(
            [Frozen] BuyingCatalogueDbContext context,
            string description,
            Organisation organisation,
            EntityFramework.Catalogue.Models.Framework framework,
            OrderService service)
        {
            framework.IsExpired = true;

            await context.Organisations.AddAsync(organisation);
            await context.Frameworks.AddAsync(framework);
            await context.SaveChangesAsync();

            await FluentActions.Invoking(async () => await service.CreateOrder(description, organisation.InternalIdentifier, OrderTypeEnum.Solution, framework.Id))
                .Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task CreateOrder_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            string description,
            Organisation organisation,
            EntityFramework.Catalogue.Models.Framework framework,
            OrderService service)
        {
            framework.IsExpired = false;

            await context.Organisations.AddAsync(organisation);
            await context.Frameworks.AddAsync(framework);
            await context.SaveChangesAsync();

            await service.CreateOrder(description, organisation.InternalIdentifier, OrderTypeEnum.Solution, framework.Id);

            var order = await context.Orders.Include(o => o.OrderingParty).FirstAsync();

            order.OrderNumber.Should().Be(1);
            order.Revision.Should().Be(1);
            order.Description.Should().Be(description);
            order.SelectedFrameworkId.Should().Be(framework.Id);
            order.OrderingParty.InternalIdentifier.Should().Be(organisation.InternalIdentifier);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AmendOrder_UpdatesDatabase(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var result = await service.AmendOrder(order.OrderingParty.InternalIdentifier, order.CallOffId);

            result.OrderNumber.Should().Be(order.OrderNumber);
            result.Revision.Should().Be(order.CallOffId.Revision + 1);
            result.OrderType.Should().Be(order.OrderType);
            result.CommencementDate.Should().Be(order.CommencementDate);
            result.Description.Should().Be(order.Description);
            result.InitialPeriod.Should().Be(order.InitialPeriod);
            result.MaximumTerm.Should().Be(order.MaximumTerm);
            result.OrderingPartyId.Should().Be(order.OrderingPartyId);
            result.SelectedFrameworkId.Should().Be(order.SelectedFrameworkId);
            result.SupplierId.Should().Be(order.SupplierId);

            result.OrderingPartyContact.Id.Should().NotBe(order.OrderingPartyContact.Id);
            result.SupplierContact.Id.Should().NotBe(order.SupplierContact.Id);

            result.OrderingPartyContact.FirstName.Should().Be(order.OrderingPartyContact.FirstName);
            result.OrderingPartyContact.LastName.Should().Be(order.OrderingPartyContact.LastName);
            result.OrderingPartyContact.Department.Should().Be(order.OrderingPartyContact.Department);
            result.OrderingPartyContact.Email.Should().Be(order.OrderingPartyContact.Email);
            result.OrderingPartyContact.Phone.Should().Be(order.OrderingPartyContact.Phone);

            result.SupplierContact.FirstName.Should().Be(order.SupplierContact.FirstName);
            result.SupplierContact.LastName.Should().Be(order.SupplierContact.LastName);
            result.SupplierContact.Department.Should().Be(order.SupplierContact.Department);
            result.SupplierContact.Email.Should().Be(order.SupplierContact.Email);
            result.SupplierContact.Phone.Should().Be(order.SupplierContact.Phone);

            result.OrderRecipients.Count.Should().Be(order.OrderRecipients.Count);
            result.OrderItems.Count.Should().Be(order.OrderItems.Count);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SoftDeleteOrder_SoftDeletedOrder(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderService service)
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            await service.SoftDeleteOrder(order.CallOffId, order.OrderingParty.InternalIdentifier);

            var updatedOrder = await context.Orders.FirstOrDefaultAsync();

            // Although soft deleted, there is a query filter on the context to exclude soft deleted orders
            updatedOrder.Should().BeNull();

            updatedOrder = await context.Orders.IgnoreQueryFilters().FirstOrDefaultAsync();
            updatedOrder.Should().Be(order);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task HardDeleteOrder_DeletesOrder(
            Contract contract,
            ImplementationPlan plan,
            ImplementationPlanMilestone milestone,
            OrderTermination orderTermination,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            plan.Milestones.Add(milestone);
            contract.ImplementationPlan = plan;
            order.Contract = contract;
            order.OrderTermination = orderTermination;

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            context.Contracts.Count().Should().Be(1);
            context.ImplementationPlans.Count().Should().Be(1);
            context.ImplementationPlanMilestones.Count().Should().Be(1);
            context.ContractFlags.Count().Should().Be(1);
            context.OrderTerminations.Count().Should().Be(1);
            context.Orders.Count().Should().Be(1);
            context.OrderDeletionApprovals.Count().Should().Be(1);
            context.OrderItems.Count().Should().Be(3);
            context.OrderItemFunding.Count().Should().Be(3);
            context.OrderItemPriceTiers.Count().Should().Be(9);
            context.OrderItemPrices.Count().Should().Be(3);
            context.OrderItemRecipients.Count().Should().Be(9);

            await service.HardDeleteOrder(order.CallOffId, order.OrderingParty.InternalIdentifier);

            context.Contracts.Should().BeEmpty();
            context.ImplementationPlans.Should().BeEmpty();
            context.ImplementationPlanMilestones.Should().BeEmpty();
            context.ContractFlags.Should().BeEmpty();
            context.OrderTerminations.Should().BeEmpty();
            context.Orders.Should().BeEmpty();
            context.OrderDeletionApprovals.Should().BeEmpty();
            context.OrderItems.Should().BeEmpty();
            context.OrderItemFunding.Should().BeEmpty();
            context.OrderItemPriceTiers.Should().BeEmpty();
            context.OrderItemPrices.Should().BeEmpty();
            context.OrderItemRecipients.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task TerminateOrder_TerminatesCurrentOrder(
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            Order order,
            DateTime terminationDate,
            string reason,
            OrderService service)
        {
            await context.Orders.AddAsync(order);

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            await service.TerminateOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, user.Id, terminationDate, reason);

            await IsTerminated(context, order.Id, terminationDate, reason);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task TerminateOrder_WithCompletedAmendment_TerminatesAllRevisions(
            Organisation organisation,
            Order originalOrder,
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser user,
            DateTime terminationDate,
            string reason,
            OrderService service)
        {
            originalOrder.Revision = 1;
            originalOrder.OrderNumber = originalOrder.ContractOrderNumber.Id;
            var amendedOrder = originalOrder.BuildAmendment(2);

            amendedOrder.Completed = DateTime.UtcNow;
            originalOrder.Completed = DateTime.UtcNow;

            var orders = new List<Order>() { originalOrder, amendedOrder };

            organisation.Orders.AddRange(orders);

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            await context.Users.AddAsync(user);

            await context.SaveChangesAsync();

            await service.TerminateOrder(amendedOrder.CallOffId, amendedOrder.OrderingParty.InternalIdentifier, user.Id, terminationDate, reason);

            await IsTerminated(context, amendedOrder.Id, terminationDate, reason);
            await IsTerminated(context, originalOrder.Id, terminationDate, reason);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task TerminateOrder_SendsFinanceCSVEmail(
            AspNetUser user,
            Order order,
            DateTime terminationDate,
            string reason,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IGovNotifyEmailService mockEmailService,
            [Frozen] ICsvService mockCsvService,
            [Frozen] IOrderPdfService mockPdfService,
            OrderMessageSettings settings)
        {
            Dictionary<string, dynamic> adminTokens = null;

            await context.Orders.AddAsync(order);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var bytes = Encoding.ASCII.GetBytes("Testing");

            mockCsvService
                .CreateFullOrderCsvAsync(order.Id, order.OrderType, Arg.Any<MemoryStream>(), false)
                .Returns(x =>
                {
                    var stream = x.ArgAt<MemoryStream>(2);
                    stream.Write(bytes);
                    return Task.CompletedTask;
                });

            mockEmailService
                .SendEmailAsync(settings.Recipient.Address, settings.OrderTerminatedAdminTemplateId, Arg.Any<Dictionary<string, dynamic>>())
                .Returns(Task.CompletedTask)
                .AndDoes((x) => adminTokens = x.ArgAt<Dictionary<string, dynamic>>(2));

            var expectedToken = NotificationClient.PrepareUpload(bytes, true);

            var service = new OrderService(
                context,
                mockCsvService,
                mockEmailService,
                mockPdfService,
                settings);

            await service.TerminateOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, user.Id, terminationDate, reason);

            adminTokens.Should().NotBeNull();
            adminTokens.Should().HaveCount(2);
            var organisationName = adminTokens.Should().ContainKey(OrderService.OrganisationNameToken).WhoseValue as string;
            var fullOrderCsv = adminTokens.Should().ContainKey(OrderService.FullOrderCsvToken).WhoseValue as JObject;
            organisationName.Should().Be(order.OrderingParty.Name);
            fullOrderCsv.Should().BeEquivalentTo(expectedToken);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task TerminateOrder_SendsUserCSVEmail(
            AspNetUser user,
            Order order,
            string email,
            DateTime terminationDate,
            string reason,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IGovNotifyEmailService mockEmailService,
            [Frozen] ICsvService mockCsvService,
            [Frozen] IOrderPdfService mockPdfService,
            OrderMessageSettings settings)
        {
            Dictionary<string, dynamic> userTokens = null;

            await context.Orders.AddAsync(order);

            user.Email = email;
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var bytes = Encoding.ASCII.GetBytes("Testing");

            mockCsvService
                .CreateFullOrderCsvAsync(order.Id, order.OrderType, Arg.Any<MemoryStream>(), false)
                .Returns(x =>
                {
                    var stream = x.ArgAt<MemoryStream>(2);
                    stream.Write(bytes);
                    return Task.CompletedTask;
                });

            mockEmailService
                .SendEmailAsync(user.Email, settings.OrderTerminatedUserTemplateId, Arg.Any<Dictionary<string, dynamic>>())
                .Returns(Task.CompletedTask)
                .AndDoes((x) => userTokens = x.ArgAt<Dictionary<string, dynamic>>(2));

            var service = new OrderService(
                context,
                mockCsvService,
                mockEmailService,
                mockPdfService,
                settings);

            var expectedOrderSummaryCsv = NotificationClient.PrepareUpload(bytes, true);

            await service.TerminateOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, user.Id, terminationDate, reason);

            userTokens.Should().NotBeNull();
            userTokens.Should().HaveCount(2);

            var orderId = userTokens.Should().ContainKey(OrderService.OrderIdToken).WhoseValue as string;
            var orderSummaryCsv = userTokens.Should().ContainKey(OrderService.OrderSummaryCsv).WhoseValue as JObject;

            orderId.Should().Be($"{order.CallOffId}");
            orderSummaryCsv.Should().BeEquivalentTo(expectedOrderSummaryCsv);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task CompleteOrder_RequestIsValid_OrderStatusUpdated(
            AspNetUser user,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.IsDeleted = false;
            order.Completed = null;
            await context.Orders.AddAsync(order);

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            await service.CompleteOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, user.Id);
            context.Orders.First(x => x.Id == order.Id).OrderStatus.Should().Be(OrderStatus.Completed);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task CompleteOrder_ContainsNoRecipients_SendsSingleCsvEmails(
            AspNetUser user,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IGovNotifyEmailService mockEmailService,
            [Frozen] ICsvService mockCsvService,
            [Frozen] IOrderPdfService mockPdfService,
            OrderMessageSettings settings)
        {
            Dictionary<string, dynamic> adminTokens = null;

            await context.Orders.AddAsync(order);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var bytes = Encoding.ASCII.GetBytes("Testing");

            mockCsvService
                .CreateFullOrderCsvAsync(order.Id, order.OrderType, Arg.Any<MemoryStream>(), false)
                .Returns(x =>
                {
                    var stream = x.ArgAt<MemoryStream>(2);
                    stream.Write(bytes);
                    return Task.CompletedTask;
                });

            mockEmailService
                .SendEmailAsync(settings.Recipient.Address, settings.SingleCsvTemplateId, Arg.Any<Dictionary<string, dynamic>>())
                .Returns(Task.CompletedTask)
                .AndDoes((x) => adminTokens = x.ArgAt<Dictionary<string, dynamic>>(2));

            var expectedToken = NotificationClient.PrepareUpload(bytes, true);

            var service = new OrderService(
                context,
                mockCsvService,
                mockEmailService,
                mockPdfService,
                settings);

            await service.CompleteOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, user.Id);

            adminTokens.Should().NotBeNull();
            adminTokens.Should().HaveCount(2);
            var organisationName = adminTokens.Should().ContainKey(OrderService.OrganisationNameToken).WhoseValue as string;
            var fullOrderCsv = adminTokens.Should().ContainKey(OrderService.FullOrderCsvToken).WhoseValue as JObject;
            organisationName.Should().Be(order.OrderingParty.Name);
            fullOrderCsv.Should().BeEquivalentTo(expectedToken);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task CompleteOrder_RequestIsValid_SendsUserEmails(
            AspNetUser user,
            Order order,
            string email,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IGovNotifyEmailService mockEmailService,
            [Frozen] ICsvService mockCsvService,
            [Frozen] IOrderPdfService mockPdfService,
            OrderMessageSettings settings)
        {
            Dictionary<string, dynamic> userTokens = null;

            order.OrderType = OrderTypeEnum.Solution;
            await context.Orders.AddAsync(order);

            user.Email = email;
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var bytes = Encoding.ASCII.GetBytes("Testing");

            mockCsvService
                .CreateFullOrderCsvAsync(order.Id, order.OrderType, Arg.Any<MemoryStream>(), false)
                .Returns(x =>
                {
                    var stream = x.ArgAt<MemoryStream>(2);
                    stream.Write(bytes);
                    return Task.CompletedTask;
                });

            mockEmailService
                .SendEmailAsync(user.Email, settings.UserTemplateId, Arg.Any<Dictionary<string, dynamic>>())
                .Returns(Task.CompletedTask)
                .AndDoes((x) => userTokens = x.ArgAt<Dictionary<string, dynamic>>(2));

            var service = new OrderService(
                context,
                mockCsvService,
                mockEmailService,
                mockPdfService,
                settings);

            var expectedOrderSummaryCsv = NotificationClient.PrepareUpload(bytes, true);

            await service.CompleteOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, user.Id);

            userTokens.Should().NotBeNull();
            userTokens.Should().HaveCount(2);

            var orderId = userTokens.Should().ContainKey(OrderService.OrderIdToken).WhoseValue as string;
            var orderSummaryCsv = userTokens.Should().ContainKey(OrderService.OrderSummaryCsv).WhoseValue as JObject;

            orderId.Should().Be($"{order.CallOffId}");
            orderSummaryCsv.Should().BeEquivalentTo(expectedOrderSummaryCsv);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task CompleteOrder_CatalogueSolution_EmailsCatalogueSolutionEmail(
            AspNetUser user,
            Order order,
            string email,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IGovNotifyEmailService mockEmailService,
            [Frozen] ICsvService mockCsvService,
            [Frozen] IOrderPdfService mockPdfService,
            OrderMessageSettings orderMessageSettings)
        {
            order.OrderType = OrderTypeEnum.Solution;
            await context.Orders.AddAsync(order);

            user.Email = email;
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var service = new OrderService(
                context,
                mockCsvService,
                mockEmailService,
                mockPdfService,
                orderMessageSettings);

            await service.CompleteOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, user.Id);

            await mockEmailService.Received().SendEmailAsync(email, orderMessageSettings.UserTemplateId, Arg.Any<Dictionary<string, dynamic>>());
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task CompleteOrder_AssociatedServiceOnly_EmailsAssociatedServiceEmail(
            AspNetUser user,
            Order order,
            string email,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IGovNotifyEmailService mockEmailService,
            [Frozen] ICsvService mockCsvService,
            [Frozen] IOrderPdfService mockPdfService,
            OrderMessageSettings orderMessageSettings)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            await context.Orders.AddAsync(order);

            user.Email = email;
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var service = new OrderService(
                context,
                mockCsvService,
                mockEmailService,
                mockPdfService,
                orderMessageSettings);

            await service.CompleteOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, user.Id);

            await mockEmailService.Received().SendEmailAsync(email, orderMessageSettings.UserAssociatedServiceTemplateId, Arg.Any<Dictionary<string, dynamic>>());
        }

        [Theory(Skip = "Temporal queries not supported in EF Core 7.")]
        [MockInMemoryDbAutoData]
        public static async Task GetOrderForSummary_CompletedOrder_ReturnsExpectedResultsAsAtCompletionDate(
            Order order,
            Supplier supplier,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            const string junk = "Junk";

            order.SupplierId = supplier.Id;
            order.Supplier = supplier;

            context.Suppliers.Add(supplier);
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var result = (await service.GetOrderForSummary(order.CallOffId, order.OrderingParty.InternalIdentifier)).Order;

            result.Supplier.Address.Should().BeEquivalentTo(supplier.Address);

            order.Complete();

            await context.SaveChangesAsync();

            supplier.Address.County += junk;
            supplier.Address.Country += junk;
            supplier.Address.Line1 += junk;
            supplier.Address.Line2 += junk;
            supplier.Address.Line3 += junk;
            supplier.Address.Line4 += junk;
            supplier.Address.Line5 += junk;
            supplier.Address.Postcode += junk;
            supplier.Address.Town += junk;

            await context.SaveChangesAsync();

            var actual = (await service.GetOrderForSummary(order.CallOffId, order.OrderingParty.InternalIdentifier)).Order;

            actual.Supplier.Address.Should().NotBeEquivalentTo(supplier.Address);
            actual.Supplier.Address.County.Should().Be(supplier.Address.County.Replace(junk, string.Empty));
            actual.Supplier.Address.Country.Should().Be(supplier.Address.Country.Replace(junk, string.Empty));
            actual.Supplier.Address.Line1.Should().Be(supplier.Address.Line1.Replace(junk, string.Empty));
            actual.Supplier.Address.Line2.Should().Be(supplier.Address.Line2.Replace(junk, string.Empty));
            actual.Supplier.Address.Line3.Should().Be(supplier.Address.Line3.Replace(junk, string.Empty));
            actual.Supplier.Address.Line4.Should().Be(supplier.Address.Line4.Replace(junk, string.Empty));
            actual.Supplier.Address.Line5.Should().Be(supplier.Address.Line5.Replace(junk, string.Empty));
            actual.Supplier.Address.Postcode.Should().Be(supplier.Address.Postcode.Replace(junk, string.Empty));
            actual.Supplier.Address.Town.Should().Be(supplier.Address.Town.Replace(junk, string.Empty));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_ReturnsExpectedPageSize(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            organisation.Orders = orders;

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            context.SaveChanges();

            (PagedList<Order> pagedOrders, IEnumerable<CallOffId> orderIds) = await service.GetPagedOrders(organisation.Id, new PageOptions("0", 2));

            orderIds.Should().BeEquivalentTo(orders.Select(x => x.CallOffId));

            pagedOrders.Items.Count.Should().Be(2);
            pagedOrders.Options.TotalNumberOfItems.Should().Be(orders.Count);

            var expected = (int)Math.Ceiling((double)orders.Count / pagedOrders.Options.PageSize);

            pagedOrders.Options.NumberOfPages.Should().Be(expected);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_SearchTerm_ReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            organisation.Orders = orders;

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            context.SaveChanges();

            var order = orders.First();
            var searchTerm = order.CallOffId.ToString();

            var result = await service.GetPagedOrders(organisation.Id, new PageOptions("0", 2), searchTerm);

            result.Orders.Items.First().Should().BeEquivalentTo(order);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_WithCompletedAmendment_ReturnsSingleRevision(
            Organisation organisation,
            Order originalOrder,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            originalOrder.OrderNumber = originalOrder.ContractOrderNumber.Id;
            originalOrder.Revision = 1;
            var amendedOrder = originalOrder.BuildAmendment(2);

            amendedOrder.Completed = DateTime.UtcNow;
            originalOrder.Completed = DateTime.UtcNow;

            var orders = new List<Order> { originalOrder, amendedOrder };
            organisation.Orders.AddRange(orders);

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            await context.SaveChangesAsync();

            (PagedList<Order> pagedOrders, _) = await service.GetPagedOrders(organisation.Id, new PageOptions("0", 10));

            pagedOrders.Items.Should().NotContain(originalOrder);
            pagedOrders.Items.Should().Contain(amendedOrder);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_WithInProgressAmendment_ReturnsAllOrders(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            var originalOrder = orders.First();
            var amendedOrder = orders.Skip(1).First();

            amendedOrder.OrderNumber = originalOrder.OrderNumber;
            originalOrder.Revision = 1;
            amendedOrder.Revision = 2;
            amendedOrder.Completed = null;
            originalOrder.Completed = DateTime.UtcNow;

            organisation.Orders.AddRange(orders);

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            await context.SaveChangesAsync();

            (PagedList<Order> pagedOrders, _) = await service.GetPagedOrders(organisation.Id, new PageOptions("0", 10));

            pagedOrders.Items.Should().Contain(originalOrder);
            pagedOrders.Items.Should().Contain(amendedOrder);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_WithNoAmendment_ReturnsOriginalOrders(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            var originalOrder = orders.First();

            originalOrder.Revision = 1;
            originalOrder.Completed = DateTime.UtcNow;

            organisation.Orders.AddRange(orders);

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            await context.SaveChangesAsync();

            (PagedList<Order> pagedOrders, _) = await service.GetPagedOrders(organisation.Id, new PageOptions("0", 10));

            pagedOrders.Items.Should().Contain(originalOrder);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrdersBySearchTerm_CallOffId_ReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            organisation.Orders = orders;

            context.Organisations.Add(organisation);
            context.Orders.AddRange(orders);

            context.SaveChanges();

            var order = orders.First();
            var searchTerm = order.CallOffId.ToString()[5..];

            var results = await service.GetOrdersBySearchTerm(organisation.Id, searchTerm);

            results.Should().NotBeEmpty();

            var actual = results.First();
            actual.Category.Should().Be(order.CallOffId.ToString());
            actual.Title.Should().Be(order.Description);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrdersBySearchTerm_Description_ReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            organisation.Orders = orders;

            context.Organisations.Add(organisation);
            context.Orders.AddRange(orders);

            context.SaveChanges();

            var order = orders.First();
            var searchTerm = order.Description[..15];

            var results = await service.GetOrdersBySearchTerm(organisation.Id, searchTerm);

            results.Should().NotBeEmpty();

            var actual = results.First();
            actual.Category.Should().Be(order.CallOffId.ToString());
            actual.Title.Should().Be(order.Description);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrdersBySearchTerm_WithCompletedAmendment_ReturnsSingleRevision(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            var originalOrder = orders.First();
            var amendedOrder = orders.Skip(1).First();

            amendedOrder.OrderNumber = originalOrder.OrderNumber;
            originalOrder.Revision = 1;
            amendedOrder.Revision = 2;
            amendedOrder.Completed = DateTime.UtcNow;
            originalOrder.Completed = DateTime.UtcNow;

            organisation.Orders = orders;

            context.Organisations.Add(organisation);
            context.Orders.AddRange(orders);

            context.SaveChanges();

            var results = await service.GetOrdersBySearchTerm(organisation.Id, originalOrder.OrderNumber.ToString());

            results.Should().NotBeEmpty();
            results.Should().ContainSingle();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrdersBySearchTerm_WithInProgressAmendment_ReturnsAllOrders(
            Organisation organisation,
            Order originalOrder,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            originalOrder.Revision = 1;
            originalOrder.OrderNumber = originalOrder.ContractOrderNumber.Id;
            var amendedOrder = originalOrder.BuildAmendment(2);

            amendedOrder.Completed = null;
            originalOrder.Completed = DateTime.UtcNow;

            var orders = new List<Order> { originalOrder, amendedOrder };

            organisation.Orders = orders;

            context.Organisations.Add(organisation);
            context.Orders.AddRange(orders);

            context.SaveChanges();

            var results = await service.GetOrdersBySearchTerm(organisation.Id, originalOrder.OrderNumber.ToString());

            results.Should().NotBeEmpty();
            results.Should().HaveCount(2);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetUserOrders_ReturnsExpectedResults(
            int userId,
            List<Order> orders,
            [Frozen] IIdentityService mockIdentityService,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            mockIdentityService
                .GetUserId()
                .Returns(userId);

            context.Orders.AddRange(orders);
            context.SaveChanges();

            var results = await service.GetUserOrders(userId);

            results.Should().BeEquivalentTo(orders);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetSolutionId_UpdatesDatabase(
            Order order,
            CatalogueItemId solutionId,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.AssociatedServicesOnlyDetails.SolutionId = null;
            order.AssociatedServicesOnlyDetails.Solution = null;

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            (await context.Orders.FirstAsync(x => x.Id == order.Id)).AssociatedServicesOnlyDetails.SolutionId.Should().BeNull();

            await service.SetSolutionId(order.OrderingParty.InternalIdentifier, order.CallOffId, solutionId);

            (await context.Orders.FirstAsync(x => x.Id == order.Id)).AssociatedServicesOnlyDetails.SolutionId.Should().Be(solutionId);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetOrderPracticeReorganisationRecipient_UpdatesDatabase(
            Order order,
            OdsOrganisation odsOrganisation,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient = null;
            order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode = null;
            context.OdsOrganisations.Add(odsOrganisation);
            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            (await context.Orders.FirstAsync(x => x.Id == order.Id)).AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode.Should().BeNull();

            await service.SetOrderPracticeReorganisationRecipient(order.OrderingParty.InternalIdentifier, order.CallOffId, odsOrganisation.Id);

            (await context.Orders.FirstAsync(x => x.Id == order.Id)).AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode.Should().Be(odsOrganisation.Id);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(FundingType.LocalFunding)]
        [MockInMemoryDbInlineAutoData(FundingType.Gpit)]
        [MockInMemoryDbInlineAutoData(FundingType.Pcarp)]
        public static async Task SetFundingSourceForForceFundedItems_FrameworkHasSingleFundingType_UpdatesDatabase(
            FundingType fundingType,
            Order order,
            OrderItem orderItem,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.SelectedFramework.FundingTypes = new List<FundingType> { fundingType };
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.OnDemand;
            orderItem.OrderItemFunding = null;
            order.OrderItems = new List<OrderItem>() { orderItem };
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.SetFundingSourceForForceFundedItems(order.OrderingParty.InternalIdentifier, order.CallOffId);

            var result = await context.Orders.FirstAsync(x => x.Id == order.Id);
            var orderItemFunding = result.OrderItems.First().OrderItemFunding;
            orderItemFunding.Should().NotBeNull();
            orderItemFunding.OrderId.Should().Be(order.Id);
            orderItemFunding.CatalogueItemId.Should().Be(orderItem.CatalogueItemId);
            orderItemFunding.OrderItemFundingType.Should().Be(fundingType.AsOrderItemFundingType());
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetFundingSourceForForceFundedItems_GpPractice_UpdatesDatabase(
            Order order,
            Organisation organisation,
            OrderItem orderItem,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            organisation.OrganisationType = OrganisationType.GP;
            order.OrderingParty = organisation;
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.OnDemand;
            orderItem.OrderItemFunding = null;
            order.OrderItems = new List<OrderItem>() { orderItem };
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.SetFundingSourceForForceFundedItems(order.OrderingParty.InternalIdentifier, order.CallOffId);

            var result = await context.Orders.FirstAsync(x => x.Id == order.Id);
            var orderItemFunding = result.OrderItems.First().OrderItemFunding;
            orderItemFunding.Should().NotBeNull();
            orderItemFunding.OrderId.Should().Be(order.Id);
            orderItemFunding.CatalogueItemId.Should().Be(orderItem.CatalogueItemId);
            orderItemFunding.OrderItemFundingType.Should().Be(OrderItemFundingType.LocalFundingOnly);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task EnsureOrderItemsForAmendment_Adds_OrderItems(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.OrderingPartyId = order.OrderingParty.Id;
            order.OrderNumber = order.ContractOrderNumber.Id;
            order.Revision = 1;
            var amendment = order.BuildAmendment(2);
            amendment.OrderItems.Clear();

            context.Orders.Add(order);
            context.Orders.Add(amendment);
            await context.SaveChangesAsync();

            await service.EnsureOrderItemsForAmendment(amendment.OrderingParty.InternalIdentifier, amendment.CallOffId);
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .Include(o => o.OrderItems)
                .FirstAsync(x => x.Id == amendment.Id);

            order.OrderItems.ForEach(i => dbOrder.Exists(i.CatalogueItemId).Should().BeTrue());
        }

        private static async Task IsTerminated(BuyingCatalogueDbContext context, int id, DateTime terminationDate, string reason)
        {
            var updatedOrder = await context.Orders
                .Include(x => x.OrderTermination)
                .FirstOrDefaultAsync(x => x.Id == id);

            updatedOrder.IsTerminated.Should().BeTrue();
            updatedOrder.OrderTermination.Should().NotBeNull();
            updatedOrder.OrderTermination.OrderId.Should().Be(id);
            updatedOrder.OrderTermination.DateOfTermination.Should().Be(terminationDate);
            updatedOrder.OrderTermination.Reason.Should().Be(reason);
        }
    }
}
