using System;
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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Notify.Client;
using NSubstitute;
using Xunit;
using EntityOdsOrganisation = NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models.OdsOrganisation;
using ServiceContractOdsOrganisation = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations.OdsOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    [SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "Skipping tests that use Temporal queries")]
    public static class OrderServiceTests
    {
        private const int CommonOrganisationId = 21;
        private const int CommonOrderNumber = 10001;
        private const string CommonOrganisationInternalIdentifier = "BB-FFGG";
        private const string CommonOrganisationExternalIdentifier = "FFGG";

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
        [MockInMemoryDbAutoData]
        public static async Task GetOrderWithSublocations_ReturnsOrder(
            Organisation organisation,
            Order order,
            List<OrderSublocation> orderSublocations,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.OrderingParty = organisation;

            orderSublocations.ForEach(x =>
            {
                x.OwnerOdsCode = organisation.ExternalIdentifier;
                x.OrderId = order.Id;
                x.SublocationOrganisation.Id = x.SublocationOdsCode;
            });

            order.OrderSublocations = orderSublocations;

            context.Add(order);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            OrderWrapper result = await service.GetOrderWithSublocations(
                order.CallOffId,
                organisation.InternalIdentifier);

            result.Order.Should()
                .BeEquivalentTo(
                    order,
                    opt => opt
                        .Excluding(m => m.OrderSublocations));
            result.Order.OrderSublocations.Should()
                .BeEquivalentTo(
                    order.OrderSublocations,
                    opt => opt.Excluding(m => m.Order).Excluding(m => m.SublocationRecipients));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrdersWithSublocationsAndSublocationRecipients_ReturnsOrder(
            Organisation organisation,
            Order order,
            List<OrderSublocation> orderSublocations,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.OrderingParty = organisation;

            orderSublocations.ForEach(x =>
            {
                x.OwnerOdsCode = organisation.ExternalIdentifier;
                x.OrderId = order.Id;
                x.SublocationOrganisation.Id = x.SublocationOdsCode;
                foreach (OrderSublocationRecipient orderSublocationRecipient in x.SublocationRecipients)
                {
                    orderSublocationRecipient.RecipientOdsOrganisation.Id =
                        orderSublocationRecipient.RecipientOdsCode;
                }
            });

            order.OrderSublocations = orderSublocations;

            context.Add(order);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            OrderWrapper result = await service.GetOrderWithSublocationsAndSublocationRecipients(
                order.CallOffId,
                organisation.InternalIdentifier
            );

            result.Should().NotBeNull();

            result.Order.Should()
                .BeEquivalentTo(
                    order,
                    opt => opt.Excluding(m => m.OrderSublocations));
            result.Order.OrderSublocations.Should()
                .BeEquivalentTo(
                    order.OrderSublocations,
                    opt => opt.Excluding(m => m.Order)
                        .Excluding(m => m.SublocationOrganisation)
                        .Excluding(m => m.SublocationRecipients));

            foreach (OrderSublocation expectedOrderSublocation in order.OrderSublocations)
            {
                ICollection<OrderSublocationRecipient> actualSublocationRecipients = result.Order.OrderSublocations
                    .First(x => x.SublocationOdsCode == expectedOrderSublocation.SublocationOdsCode)
                    .SublocationRecipients;

                expectedOrderSublocation.SublocationRecipients.Should()
                    .BeEquivalentTo(
                        actualSublocationRecipients,
                        opt => opt.Excluding(m => m.Order).Excluding(m => m.ParentSublocation));
            }
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(false, false)]
        [MockInMemoryDbInlineAutoData(true, true)]
        public static async Task GetOrderHasAnySublocations_ReturnsBool(
            bool addSublocation,
            bool expectedResult,
            Order order,
            Organisation organisation,
            OrderSublocation orderSublocation,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.OrderingParty = organisation;

            if (addSublocation)
            {
                order.OrderSublocations.Add(orderSublocation);
            }

            context.Add(order);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var result = await service.GetOrderHasAnySublocations(order.CallOffId, organisation.InternalIdentifier);

            Assert.Equal(expectedResult, result);
        }

        public static IEnumerable<object[]> OrderCountData()
        {
            return
            [
                [
                    CommonOrganisationFactory(21),
                    CommonOrderFactory(
                        23,
                        21,
                        6678,
                        0,
                        [
                            CommonOrderSublocationFactory(
                                23,
                                "XXXX",
                                [
                                    CommonOrderSublocationRecipientFactory(23, "AAAA", "XXXX"),
                                    CommonOrderSublocationRecipientFactory(23, "AAAB", "XXXX"),
                                    CommonOrderSublocationRecipientFactory(23, "AAAC", "XXXX"),
                                ]
                            ),
                            CommonOrderSublocationFactory(
                                23,
                                "XXXY",
                                [
                                    CommonOrderSublocationRecipientFactory(23, "BAAA", "XXXY"),
                                    CommonOrderSublocationRecipientFactory(23, "BAAB", "XXXY"),
                                    CommonOrderSublocationRecipientFactory(23, "BAAC", "XXXY"),
                                ]),
                            CommonOrderSublocationFactory(
                                23,
                                "XXXZ",
                                [
                                    CommonOrderSublocationRecipientFactory(23, "CAAA", "XXXZ"),
                                    CommonOrderSublocationRecipientFactory(23, "CAAB", "XXXZ"),
                                ]),
                        ]),
                    8,
                ],
                [
                    CommonOrganisationFactory(31),
                    CommonOrderFactory(
                        76,
                        31,
                        8192,
                        0,
                        [
                            CommonOrderSublocationFactory(
                                76,
                                "ZXXX",
                                [
                                ]),
                            CommonOrderSublocationFactory(
                                76,
                                "ZXXY",
                                [
                                ]),
                            CommonOrderSublocationFactory(
                                76,
                                "ZXXZ",
                                [
                                ]),
                        ]),
                    0,
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(OrderCountData))]
        public static async Task GetOrderTotalRecipientCount_ReturnsInt(
            Organisation organisation,
            Order order,
            int expectedRecipientCount,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.OrderingParty = organisation;

            context.Add(order);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var result = await service.GetOrderTotalRecipientCount(order.CallOffId, organisation.InternalIdentifier);

            Assert.Equal(expectedRecipientCount, result);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData("", true, typeof(ArgumentException))]
        [MockInMemoryDbInlineAutoData(null, true, typeof(ArgumentNullException))]
        [MockInMemoryDbInlineAutoData("MY-ORG-ID", false, typeof(ArgumentException))]
        public static async Task SetSublocations_RejectsNullParams(
            string internalOrgId,
            bool populateSublocations,
            Type expectedExceptionType,
            HashSet<string> orderSublocations,
            OrderService service)
        {
            var callOffId = new CallOffId(10001, 0);

            Exception exception = await Record.ExceptionAsync(async () =>
            {
                if (!populateSublocations)
                {
                    await service.SetSublocations(
                        callOffId,
                        internalOrgId,
                        []);
                }
                else
                {
                    await service.SetSublocations(
                        callOffId,
                        internalOrgId,
                        orderSublocations);
                }
            });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(expectedExceptionType);
        }

        public static IEnumerable<object[]> SetSublocationsSingleOrderNotValidData()
        {
            Order completeOrder = CommonOrderFactory(
                45,
                32,
                7864,
                0,
                [CommonOrderSublocationFactory(45, "XXXA", [], true)]);
            completeOrder.Completed = new DateTime(2024, 01, 03);

            Order terminatedOrder = CommonOrderFactory(
                11,
                76,
                4312,
                0,
                [CommonOrderSublocationFactory(11, "XXXB", [], true)]);
            terminatedOrder.IsTerminated = true;

            Order deletedOrder = CommonOrderFactory(
                51,
                55,
                9841,
                0,
                [CommonOrderSublocationFactory(51, "XXXC", [], true)]);
            deletedOrder.IsDeleted = true;

            Order expiredOrder = CommonOrderFactory(
                36,
                66,
                4327,
                0,
                [CommonOrderSublocationFactory(36, "XXXD", [], true)]);
            expiredOrder.CommencementDate = new DateTime(2024, 01, 01);
            expiredOrder.MaximumTerm = 3;

            var addHashSet = new HashSet<string> { "XXYY" };

            return
            [
                [
                    CommonOrganisationFactory(32), completeOrder, addHashSet,
                    "Sublocations cannot be edited for this order.",
                ],
                [
                    CommonOrganisationFactory(76), terminatedOrder,

                    addHashSet, "Sublocations cannot be edited for this order.",
                ],
                [
                    CommonOrganisationFactory(55), deletedOrder,
                    addHashSet,
                    "Sequence contains no elements", // Filter at the order entity level will prevent order being included
                ],

                [
                    CommonOrganisationFactory(66), expiredOrder,
                    addHashSet, "Sublocations cannot be edited for this order.",
                ],

                [
                    CommonOrganisationFactory(78), CommonOrderFactory(
                        61,
                        78,
                        0,
                        0,
                        [
                            CommonOrderSublocationFactory(
                                61,
                                "XXXZ",
                                [],
                                true),
                        ]
                    ),
                    addHashSet,
                    "One or more requested Ids not found or not valid for this organisation.",
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(SetSublocationsSingleOrderNotValidData))]
        public static async Task SetSublocations_RejectsInvalidOperations(
            Organisation organisation,
            Order order,
            HashSet<string> sublocationOdsCodes,
            string expectedMessage,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.OrderingParty = organisation;

            context.Add(order);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            Exception exception = await Record.ExceptionAsync(async () =>
            {
                await service.SetSublocations(
                    order.CallOffId,
                    organisation.InternalIdentifier,
                    sublocationOdsCodes);
            });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(typeof(InvalidOperationException));
            exception!.Message.Should().Be(expectedMessage);
        }

        public static IEnumerable<object[]> SetSublocationsNotMostRecentRevisionInvalid()
        {
            return
            [
                [
                    CommonOrganisationFactory(78),
                    new List<Order>
                    {
                        CommonOrderFactory(45, 78, 5555, 0, [CommonOrderSublocationFactory(45, "XXXA", [], true)]),
                        CommonOrderFactory(87, 78, 5555, 1, [CommonOrderSublocationFactory(87, "XXXA", [])]),
                        CommonOrderFactory(92, 78, 5555, 2, [CommonOrderSublocationFactory(92, "XXXA", [])]),
                    },
                    new HashSet<string> { "XXXA" },
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(SetSublocationsNotMostRecentRevisionInvalid))]
        public static async Task SetSublocations_RejectsNotMostRecentOrder(
            Organisation organisation,
            List<Order> orders,
            HashSet<string> sublocationOdsCodes,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            context.Add(organisation);
            context.AddRange(orders);

            Order workingOrder = orders.First(x => x.Revision == 1);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            Exception exception = await Record.ExceptionAsync(async () =>
            {
                await service.SetSublocations(
                    workingOrder.CallOffId,
                    organisation.InternalIdentifier,
                    sublocationOdsCodes);
            });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(typeof(InvalidOperationException));
            exception!.Message.Should().Be("Can only set sublocations on the most recent order.");
        }

        public static IEnumerable<object[]> SetSublocationsData()
        {
            return
            [
                // Adds
                [
                    CommonOrganisationFactory(78), CommonOrderFactory(33, 78, 6887, 0, []),
                    new List<EntityOdsOrganisation>
                    {
                        CommonEntityOdsOrganisationFactory("XXXX"),
                        CommonEntityOdsOrganisationFactory("XXXY"),
                        CommonEntityOdsOrganisationFactory("XXXZ"),
                    },
                    new List<ServiceContractOdsOrganisation>
                    {
                        CommonServiceContractOdsOrganisationFactory("XXXX"),
                        CommonServiceContractOdsOrganisationFactory("XXXY"),
                        CommonServiceContractOdsOrganisationFactory("XXXZ"),
                    },
                    new HashSet<string> { "XXXX", "XXXY" },
                    new List<OrderSublocation>
                    {
                        CommonOrderSublocationFactory(33, "XXXX"), CommonOrderSublocationFactory(33, "XXXY"),
                    },
                ],

                // Removes
                [
                    CommonOrganisationFactory(31),
                    CommonOrderFactory(
                        467,
                        31,
                        6443,
                        0,
                        [
                            CommonOrderSublocationFactory(467, "YXXA"), CommonOrderSublocationFactory(467, "YXXB"),
                            CommonOrderSublocationFactory(467, "YXXC"),
                        ]),
                    new List<EntityOdsOrganisation>
                    {
                        CommonEntityOdsOrganisationFactory("YXXA"),
                        CommonEntityOdsOrganisationFactory("YXXB"),
                        CommonEntityOdsOrganisationFactory("YXXC"),
                    },
                    new List<ServiceContractOdsOrganisation>
                    {
                        CommonServiceContractOdsOrganisationFactory("YXXA"),
                        CommonServiceContractOdsOrganisationFactory("YXXB"),
                        CommonServiceContractOdsOrganisationFactory("YXXC"),
                    },
                    new HashSet<string> { "YXXB", "YXXC" },
                    new List<OrderSublocation>
                    {
                        CommonOrderSublocationFactory(467, "YXXB"), CommonOrderSublocationFactory(467, "YXXC"),
                    },
                ],

                // Adds and removes

                [
                    CommonOrganisationFactory(87),
                    CommonOrderFactory(
                        721,
                        87,
                        55,
                        0,
                        [
                            CommonOrderSublocationFactory(721, "ZXXA"), CommonOrderSublocationFactory(721, "ZXXB"),
                            CommonOrderSublocationFactory(721, "ZXXC"),
                        ]),
                    new List<EntityOdsOrganisation>
                    {
                        CommonEntityOdsOrganisationFactory("ZXXA"),
                        CommonEntityOdsOrganisationFactory("ZXXB"),
                        CommonEntityOdsOrganisationFactory("ZXXC"),
                        CommonEntityOdsOrganisationFactory("ZXXD"),
                        CommonEntityOdsOrganisationFactory("ZXXE"),
                        CommonEntityOdsOrganisationFactory("ZXXF"),
                    },
                    new List<ServiceContractOdsOrganisation>
                    {
                        CommonServiceContractOdsOrganisationFactory("ZXXA"),
                        CommonServiceContractOdsOrganisationFactory("ZXXB"),
                        CommonServiceContractOdsOrganisationFactory("ZXXC"),
                        CommonServiceContractOdsOrganisationFactory("ZXXD"),
                        CommonServiceContractOdsOrganisationFactory("ZXXE"),
                        CommonServiceContractOdsOrganisationFactory("ZXXF"),
                    },
                    new HashSet<string> { "ZXXB", "ZXXC", "ZXXD", "ZXXE" },
                    new List<OrderSublocation>
                    {
                        CommonOrderSublocationFactory(721, "ZXXB"),
                        CommonOrderSublocationFactory(721, "ZXXC"),
                        CommonOrderSublocationFactory(721, "ZXXD"),
                        CommonOrderSublocationFactory(721, "ZXXE"),
                    },
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(SetSublocationsData))]
        public static async Task SetSublocations_SetsSublocations(
            Organisation organisation,
            Order order,
            List<EntityOdsOrganisation> validSublocationsAsEntityModels,
            List<ServiceContractOdsOrganisation> validSublocationsAsServiceModels,
            HashSet<string> setSublocationOdsCodes,
            List<OrderSublocation> expectedOrderSublocations,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOdsService odsService,
            OrderService service)
        {
            order.OrderingParty = organisation;

            context.AddRange(validSublocationsAsEntityModels);
            context.Add(organisation);
            context.Add(order);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            odsService.GetSublocationsByParentOdsCode(organisation.ExternalIdentifier)
                .Returns(validSublocationsAsServiceModels);

            await service.SetSublocations(
                order.CallOffId,
                organisation.InternalIdentifier,
                setSublocationOdsCodes);

            OrderWrapper actualOrder = await service.GetOrderWithSublocations(
                order.CallOffId,
                organisation.InternalIdentifier);

            actualOrder.Order.OrderSublocations.Should()
                .BeEquivalentTo(
                    expectedOrderSublocations,
                    opt => opt.WithoutStrictOrdering()
                        .Excluding(m => m.Order)
                        .Excluding(m => m.SublocationOrganisation)
                        .Excluding(m => m.SublocationRecipients));
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

            result.FlattenedRecipients.Count().Should().Be(order.FlattenedRecipients.Count());
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
            [Frozen] IOdsService mockOdsService,
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
                mockOdsService,
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
            [Frozen] IOdsService mockOdsService,
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
                mockOdsService,
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
            [Frozen] IOdsService mockOdsService,
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
                mockOdsService,
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
            [Frozen] IOdsService mockOdsService,
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
                mockOdsService,
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
            [Frozen] IOdsService mockOdsService,
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
                mockOdsService,
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
            [Frozen] IOdsService mockOdsService,
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
                mockOdsService,
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
            EntityOdsOrganisation odsOrganisation,
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

        private static Organisation CommonOrganisationFactory(int customId = 0)
        {
            return new Organisation
            {
                Id = customId == 0 ? CommonOrganisationId : customId,
                InternalIdentifier = CommonOrganisationInternalIdentifier,
                ExternalIdentifier = CommonOrganisationExternalIdentifier,
                Name = "A Local ICB",
            };
        }

        private static Order CommonOrderFactory(
            int customId = 0,
            int customOrganisationId = 0,
            int customOrderNumber = 0,
            int customRevision = 0,
            ICollection<OrderSublocation> orderSublocations = null)
        {
            var random = new Random();
            return new Order
            {
                Id = customId == 0 ? random.Next() : customId,
                OrderNumber = customOrderNumber == 0 ? CommonOrderNumber : customOrderNumber,
                Revision = customRevision,
                Description = $"An order {customId}",
                OrderingPartyId = customOrganisationId == 0 ? CommonOrganisationId : customOrganisationId,
                SelectedFramework = new EntityFramework.Catalogue.Models.Framework { Id = random.Next().ToString() },
                OrderSublocations = orderSublocations,
            };
        }

        private static OrderSublocation CommonOrderSublocationFactory(
            int orderId,
            string sublocationOdsCode,
            List<OrderSublocationRecipient> sublocationRecipients = null,
            bool hasOrganisation = false)
        {
            return new OrderSublocation
            {
                OrderId = orderId,
                SublocationOdsCode = sublocationOdsCode,
                OwnerOdsCode = CommonOrganisationExternalIdentifier,
                SublocationRecipients = sublocationRecipients,
                SublocationOrganisation =
                    hasOrganisation ? CommonEntityOdsOrganisationFactory(sublocationOdsCode) : null,
            };
        }

        private static OrderSublocationRecipient CommonOrderSublocationRecipientFactory(
            int orderId,
            string recipientOdsCode,
            string parentSublocationOdsCode,
            bool hasOrganisation = false)
        {
            return new OrderSublocationRecipient
            {
                OrderId = orderId,
                RecipientOdsCode = recipientOdsCode,
                ParentSublocationOdsCode = parentSublocationOdsCode,
                RecipientOdsOrganisation =
                    hasOrganisation ? CommonEntityOdsOrganisationFactory(recipientOdsCode) : null,
            };
        }

        private static EntityOdsOrganisation CommonEntityOdsOrganisationFactory(string id)
        {
            return new EntityOdsOrganisation { Id = id, Name = $"An organisation - {id}", IsActive = true };
        }

        private static ServiceContractOdsOrganisation CommonServiceContractOdsOrganisationFactory(string id)
        {
            return new ServiceContractOdsOrganisation
            {
                OdsCode = id, OrganisationName = $"An organisation - {id}", IsActive = true,
            };
        }
    }
}
