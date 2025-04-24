using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;
using EntityOdsOrganisation = NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models.OdsOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderSublocationServiceTests
    {
        private const int CommonOrganisationId = 21;
        private const int CommonOrderId = 10001;
        private const string CommonOrganisationInternalIdentifier = "BB-FFGG";
        private const string CommonOrganisationExternalIdentifier = "FFGG";

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrderSublocationWithRecipients_ReturnsOrderSublocation(
            Organisation organisation,
            Order order,
            OrderSublocation sublocation,
            EntityOdsOrganisation sublocationOrganisation,
            List<OrderSublocationRecipient> orderSublocationRecipients,
            [Frozen] BuyingCatalogueDbContext context,
            OrderSublocationService service)
        {
            order.OrderingParty = organisation;

            sublocation.Order = order;
            sublocation.OwnerOdsCode = organisation.ExternalIdentifier;
            sublocation.SublocationOrganisation = sublocationOrganisation;

            orderSublocationRecipients.ForEach(x =>
            {
                x.RecipientOdsOrganisation = CommonEntityOdsOrganisationFactory(x.RecipientOdsCode);
            });
            sublocation.SublocationRecipients = orderSublocationRecipients;

            context.Add(sublocation);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            OrderSublocation actualSublocation = await service.GetOrderSublocationWithRecipients(
                organisation.ExternalIdentifier,
                order.Id,
                sublocation.SublocationOdsCode);

            actualSublocation.Should().NotBeNull();

            actualSublocation.Should()
                .BeEquivalentTo(
                    sublocation,
                    opt => opt.Excluding(m => m.Order)
                        .Excluding(m => m.SublocationOrganisation)
                        .Excluding(m => m.SublocationRecipients));
            actualSublocation.SublocationRecipients.Should()
                .BeEquivalentTo(
                    orderSublocationRecipients,
                    opt => opt.Excluding(m => m.Order)
                        .Excluding(m => m.RecipientOdsOrganisation)
                        .Excluding(m => m.ParentSublocation));
        }

        public static IEnumerable<object[]> OrderSublocationsWithRecipientsForCount()
        {
            return
            [
                [
                    CommonOrganisationFactory(22), CommonOrderFactory(654, 22),
                    CommonOrderSublocationFactory("XXXX", []), 0,
                ],

                [
                    CommonOrganisationFactory(65), CommonOrderFactory(621, 65),
                    CommonOrderSublocationFactory(
                        "XXXY",
                        [
                            CommonOrderSublocationRecipientFactory("BAAA", "XXXY", 621),
                            CommonOrderSublocationRecipientFactory("BAAB", "XXXY", 621),
                        ]),
                    2,
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(OrderSublocationsWithRecipientsForCount))]
        public static async Task GetCountForOrderSublocationRecipients_ReturnsCount(
            Organisation organisation,
            Order order,
            OrderSublocation sublocation,
            int expectedCount,
            [Frozen] BuyingCatalogueDbContext context,
            OrderSublocationService service)
        {
            order.OrderingParty = organisation;

            sublocation.Order = order;
            sublocation.OwnerOdsCode = organisation.ExternalIdentifier;

            context.Add(sublocation);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var count = await service.GetCountForOrderSublocationRecipients(
                organisation.ExternalIdentifier,
                order.Id,
                sublocation.SublocationOdsCode);

            Assert.Equal(expectedCount, count);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData("", 5, "MY-RECIPIENT-ID", true, typeof(ArgumentException))]
        [MockInMemoryDbInlineAutoData(null, 5, "MY-RECIPIENT-ID", true, typeof(ArgumentNullException))]
        [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, "", true, typeof(ArgumentException))]
        [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, null, true, typeof(ArgumentNullException))]
        [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, "MY-RECIPIENT-ID", false, typeof(ArgumentException))]
        public static async Task SetSublocationRecipients_RejectsNullArguments(
            string parentOdsCode,
            int orderId,
            string sublocationOdsCode,
            bool populateOdsCodes,
            Type expectedExceptionType,
            HashSet<string> odsCodes,
            OrderSublocationService service)
        {
            Exception exception = await Record.ExceptionAsync(async () =>
            {
                if (!populateOdsCodes)
                {
                    await service.SetSublocationRecipients(
                        parentOdsCode,
                        orderId,
                        sublocationOdsCode,
                        new HashSet<string>());
                }
                else
                {
                    await service.SetSublocationRecipients(
                        parentOdsCode,
                        orderId,
                        sublocationOdsCode,
                        odsCodes);
                }
            });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(expectedExceptionType);
        }

        public static IEnumerable<object[]> SetSublocationRecipientsNotMostRecentRevisionInvalid()
        {
            return
            [
                [
                    CommonOrganisationFactory(45), new List<Order>
                    {
                        new()
                        {
                            Id = 25556,
                            OrderNumber = 10001,
                            Revision = 1,
                            Description = $"My order {10001}",
                            OrderingPartyId = 45,
                            OrderSublocations = [CommonOrderSublocationFactory("XXXA", [], false, 25556)],
                        },
                        new()
                        {
                            Id = 7566,
                            OrderNumber = 10001,
                            Revision = 2,
                            Description = $"My order {10001}",
                            OrderingPartyId = 45,
                            OrderSublocations = [CommonOrderSublocationFactory("XXXA", [], true, 7566)],
                        },
                        new()
                        {
                            Id = 9482,
                            OrderNumber = 10001,
                            Revision = 3,
                            Description = $"My order {10001}",
                            OrderingPartyId = 45,
                            OrderSublocations = [CommonOrderSublocationFactory("XXXA", [], false, 9482)],
                        },
                    },
                    new HashSet<string> { "AAAA" },
                    CommonServiceRecipientFactory("AAAA", "XXXA"),
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(SetSublocationRecipientsNotMostRecentRevisionInvalid))]
        public static async Task SetSublocationRecipients_RejectsNotMostRecentOrder(
            Organisation organisation,
            List<Order> orders,
            HashSet<string> recipientOdsCodes,
            ServiceRecipient serviceRecipient,
            [Frozen] IOdsService odsService,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService orderService,
            OrderSublocationService service)
        {
            context.AddRange(orders);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            Order workingOrder = orders[^2];

            odsService.GetServiceRecipientsBySublocation(workingOrder.OrderSublocations.First().SublocationOdsCode)
                .Returns([serviceRecipient]);

            orderService.HasSubsequentRevisions(workingOrder.CallOffId).Returns(true);

            Exception exception = await Record.ExceptionAsync(async () =>
            {
                await service.SetSublocationRecipients(
                    organisation.ExternalIdentifier,
                    workingOrder.Id,
                    workingOrder.OrderSublocations.First().SublocationOdsCode,
                    recipientOdsCodes);
            });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(typeof(InvalidOperationException));
            exception!.Message.Should().Be("Can only set sublocation recipients on the most recent order.");
        }

        public static IEnumerable<object[]> SetSublocationRecipientsSingleOrderNotValidData()
        {
            Order completeOrder = CommonOrderFactory(32, 45);
            completeOrder.Completed = new DateTime(2024, 01, 03);

            Order terminatedOrder = CommonOrderFactory(76, 11);
            terminatedOrder.IsTerminated = true;

            Order deletedOrder = CommonOrderFactory(55, 51);
            deletedOrder.IsDeleted = true;

            Order expiredOrder = CommonOrderFactory(66, 36);
            expiredOrder.CommencementDate = new DateTime(2024, 01, 01);
            expiredOrder.MaximumTerm = 3;

            return
            [
                [
                    CommonOrganisationFactory(45), completeOrder,
                    CommonOrderSublocationFactory("XXXA", [], true, 32),
                    new HashSet<string> { "AAAA" }, "Sublocations cannot be edited for this order.",
                ],
                [
                    CommonOrganisationFactory(11), terminatedOrder,
                    CommonOrderSublocationFactory("XXXB", [], true, 76),
                    new HashSet<string> { "AAAA" }, "Sublocations cannot be edited for this order.",
                ],
                [
                    CommonOrganisationFactory(51), deletedOrder,
                    CommonOrderSublocationFactory("XXXC", [], true, 55),
                    new HashSet<string> { "AAAA" },
                    "Sequence contains no elements", // Filter at the order entity level will prevent order being included
                ],

                [
                    CommonOrganisationFactory(36), expiredOrder,
                    CommonOrderSublocationFactory("XXXD", [], true, 66),
                    new HashSet<string> { "AAAA" }, "Sublocations cannot be edited for this order.",
                ],

                [
                    CommonOrganisationFactory(61), CommonOrderFactory(78, 61),
                    CommonOrderSublocationFactory(
                        "XXXZ",
                        [],
                        true,
                        78),
                    new HashSet<string> { "AAAA" },
                    "One or more requested Ids not found or not valid for this sublocation.",
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(SetSublocationRecipientsSingleOrderNotValidData))]
        public static async Task SetSublocationRecipients_RejectsOtherInvalidOperations(
            Organisation organisation,
            Order order,
            OrderSublocation orderSublocation,
            HashSet<string> recipientOdsCodes,
            string expectedMessage,
            [Frozen] IOdsService odsService,
            [Frozen] BuyingCatalogueDbContext context,
            OrderSublocationService service)
        {
            order.OrderingParty = organisation;

            context.Add(orderSublocation);
            context.Add(order);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            odsService.GetServiceRecipientsBySublocation(orderSublocation.SublocationOdsCode)
                .Returns([]);

            Exception exception = await Record.ExceptionAsync(async () =>
            {
                await service.SetSublocationRecipients(
                    organisation.ExternalIdentifier,
                    order.Id,
                    orderSublocation.SublocationOdsCode,
                    recipientOdsCodes);
            });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(typeof(InvalidOperationException));
            exception!.Message.Should().Be(expectedMessage);
        }

        public static IEnumerable<object[]> SetSublocationRecipientsData()
        {
            return
            [
                // Adds
                [
                    CommonOrganisationFactory(45), CommonOrderFactory(32, 45),
                    CommonOrderSublocationFactory("XXXX", [], true, 32),
                    new List<EntityOdsOrganisation>
                    {
                        CommonEntityOdsOrganisationFactory("AAAA"),
                        CommonEntityOdsOrganisationFactory("AAAB"),
                        CommonEntityOdsOrganisationFactory("AAAC"),
                    },
                    new List<ServiceRecipient>
                    {
                        CommonServiceRecipientFactory("AAAA", "XXXX"),
                        CommonServiceRecipientFactory("AAAB", "XXXX"),
                        CommonServiceRecipientFactory("AAAC", "XXXX"),
                    },
                    new HashSet<string> { "AAAA", "AAAB" },
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [
                            CommonOrderSublocationRecipientFactory("AAAA", "XXXX", 32, true),
                            CommonOrderSublocationRecipientFactory("AAAB", "XXXX", 32, true),
                        ],
                        true,
                        32),
                ],

                // Removes
                [
                    CommonOrganisationFactory(75), CommonOrderFactory(14, 75),
                    CommonOrderSublocationFactory(
                        "YXXX",
                        [
                            CommonOrderSublocationRecipientFactory("BAAA", "YXXX", 14),
                            CommonOrderSublocationRecipientFactory("BAAB", "YXXX", 14),
                            CommonOrderSublocationRecipientFactory("BAAC", "YXXX", 14),
                        ],
                        true,
                        14),

                    new List<EntityOdsOrganisation>
                    {
                        CommonEntityOdsOrganisationFactory("BAAA"),
                        CommonEntityOdsOrganisationFactory("BAAB"),
                        CommonEntityOdsOrganisationFactory("BAAC"),
                    },
                    new List<ServiceRecipient>
                    {
                        CommonServiceRecipientFactory("BAAA", "YXXX"),
                        CommonServiceRecipientFactory("BAAB", "YXXX"),
                        CommonServiceRecipientFactory("BAAC", "YXXX"),
                    },
                    new HashSet<string> { "BAAA", "BAAB" },
                    CommonOrderSublocationFactory(
                        "YXXX",
                        [
                            CommonOrderSublocationRecipientFactory("BAAA", "YXXX", 14, true),
                            CommonOrderSublocationRecipientFactory("BAAB", "YXXX", 14, true),
                        ],
                        true,
                        14),
                ],

                // Adds and removes
                [
                    CommonOrganisationFactory(81), CommonOrderFactory(9, 81),
                    CommonOrderSublocationFactory(
                        "ZXXX",
                        [
                            CommonOrderSublocationRecipientFactory("CAAA", "ZXXX", 9),
                            CommonOrderSublocationRecipientFactory("CAAB", "ZXXX", 9),
                            CommonOrderSublocationRecipientFactory("CAAC", "ZXXX", 9),
                        ],
                        true,
                        9),

                    new List<EntityOdsOrganisation>
                    {
                        CommonEntityOdsOrganisationFactory("CAAA"),
                        CommonEntityOdsOrganisationFactory("CAAB"),
                        CommonEntityOdsOrganisationFactory("CAAC"),
                        CommonEntityOdsOrganisationFactory("CAAD"),
                        CommonEntityOdsOrganisationFactory("CAAE"),
                        CommonEntityOdsOrganisationFactory("CAAF"),
                    },
                    new List<ServiceRecipient>
                    {
                        CommonServiceRecipientFactory("CAAA", "ZXXX"),
                        CommonServiceRecipientFactory("CAAB", "ZXXX"),
                        CommonServiceRecipientFactory("CAAC", "ZXXX"),
                        CommonServiceRecipientFactory("CAAD", "ZXXX"),
                        CommonServiceRecipientFactory("CAAE", "ZXXX"),
                        CommonServiceRecipientFactory("CAAF", "ZXXX"),
                    },
                    new HashSet<string> { "CAAA", "CAAB", "CAAD", "CAAF" },
                    CommonOrderSublocationFactory(
                        "ZXXX",
                        [
                            CommonOrderSublocationRecipientFactory("CAAA", "ZXXX", 9, true),
                            CommonOrderSublocationRecipientFactory("CAAB", "ZXXX", 9, true),
                            CommonOrderSublocationRecipientFactory("CAAD", "ZXXX", 9, true),
                            CommonOrderSublocationRecipientFactory("CAAF", "ZXXX", 9, true),
                        ],
                        true,
                        9),
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(SetSublocationRecipientsData))]
        public static async Task SetSublocationRecipients_SetsAsExpected(
            Organisation organisation,
            Order order,
            OrderSublocation workingOrderSublocation,
            List<EntityOdsOrganisation> validSublocationRecipientsAsEntityModels,
            List<ServiceRecipient> validSublocationRecipientsAsServiceModels,
            HashSet<string> addSublocationRecipientsOdsCodes,
            OrderSublocation expectedOrderSublocation,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOdsService odsService,
            OrderSublocationService service)
        {
            order.OrderingParty = organisation;

            context.AddRange(validSublocationRecipientsAsEntityModels);
            context.Add(workingOrderSublocation);
            context.Add(order);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            odsService.GetServiceRecipientsBySublocation(workingOrderSublocation.SublocationOdsCode)
                .Returns(validSublocationRecipientsAsServiceModels);

            await service.SetSublocationRecipients(
                organisation.ExternalIdentifier,
                order.Id,
                workingOrderSublocation.SublocationOdsCode,
                addSublocationRecipientsOdsCodes);

            OrderSublocation actualOrderSublocation = await service.GetOrderSublocationWithRecipients(
                organisation.ExternalIdentifier,
                order.Id,
                workingOrderSublocation.SublocationOdsCode);

            actualOrderSublocation.Should()
                .BeEquivalentTo(
                    expectedOrderSublocation,
                    opt => opt.WithoutStrictOrdering()
                        .Excluding(m => m.Order)
                        .Excluding(m => m.SublocationOrganisation)
                        .Excluding(m => m.SublocationRecipients));

            foreach (OrderSublocationRecipient expectedRecipient in expectedOrderSublocation
                         .SublocationRecipients)
            {
                OrderSublocationRecipient actualRecipient =
                    actualOrderSublocation.SublocationRecipients.First(x =>
                        x.RecipientOdsCode == expectedRecipient.RecipientOdsCode);

                actualRecipient.Should()
                    .BeEquivalentTo(
                        expectedRecipient,
                        opt => opt.Excluding(m => m.Order).Excluding(m => m.ParentSublocation));
            }
        }

        private static Organisation CommonOrganisationFactory(int customId = 0)
        {
            return new Organisation
            {
                Id = customId == 0 ? CommonOrderId : customId,
                InternalIdentifier = CommonOrganisationInternalIdentifier,
                ExternalIdentifier = CommonOrganisationExternalIdentifier,
                Name = "A Local ICB",
            };
        }

        private static Order CommonOrderFactory(int customId = 0, int customOrganisationId = 0)
        {
            return new Order
            {
                Id = customId == 0 ? CommonOrderId : customId,
                OrderNumber = customId,
                Revision = 1,
                Description = $"My order {customId}",
                OrderingPartyId = customOrganisationId == 0 ? CommonOrganisationId : customOrganisationId,
            };
        }

        private static OrderSublocation CommonOrderSublocationFactory(
            string sublocationOdsCode,
            List<OrderSublocationRecipient> sublocationRecipients = null,
            bool hasOrganisation = false,
            int customOrderId = 0)
        {
            return new OrderSublocation
            {
                OrderId = customOrderId == 0 ? CommonOrderId : customOrderId,
                SublocationOdsCode = sublocationOdsCode,
                OwnerOdsCode = CommonOrganisationExternalIdentifier,
                SublocationRecipients = sublocationRecipients,
                SublocationOrganisation =
                    hasOrganisation ? CommonEntityOdsOrganisationFactory(sublocationOdsCode) : null,
            };
        }

        private static OrderSublocationRecipient CommonOrderSublocationRecipientFactory(
            string recipientOdsCode,
            string parentSublocationOdsCode,
            int orderId = 0,
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

        private static ServiceRecipient CommonServiceRecipientFactory(string orgId, string locationOrgId)
        {
            return new ServiceRecipient { OrgId = orgId, LocationOrgId = locationOrgId };
        }
    }
}
