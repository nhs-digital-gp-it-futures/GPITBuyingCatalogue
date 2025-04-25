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
        private const int CommonOrderNumber = 10001;
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
                    CommonOrganisationFactory(22), CommonOrderFactory(22, 654),
                    CommonOrderSublocationFactory("XXXX", []), 0,
                ],

                [
                    CommonOrganisationFactory(65), CommonOrderFactory(65, 621),
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
                    CommonOrganisationFactory(45),
                    CommonOrderFactory(45, 0, 0, 0, [CommonOrderSublocationFactory("XXXA", [], true)]),

                    new HashSet<string> { "AAAA" },
                    CommonServiceRecipientFactory("AAAA", "XXXA"),
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(SetSublocationRecipientsNotMostRecentRevisionInvalid))]
        public static async Task SetSublocationRecipients_RejectsNotMostRecentOrder(
            Organisation organisation,
            Order order,
            HashSet<string> recipientOdsCodes,
            ServiceRecipient serviceRecipient,
            [Frozen] IOdsService odsService,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService orderService,
            OrderSublocationService service)
        {
            context.Add(order);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            odsService.GetServiceRecipientsBySublocation(order.OrderSublocations.First().SublocationOdsCode)
                .Returns([serviceRecipient]);

            orderService.HasSubsequentRevisions(order.CallOffId).Returns(true);

            Exception exception = await Record.ExceptionAsync(async () =>
            {
                await service.SetSublocationRecipients(
                    organisation.ExternalIdentifier,
                    order.Id,
                    order.OrderSublocations.First().SublocationOdsCode,
                    recipientOdsCodes);
            });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(typeof(InvalidOperationException));
            exception!.Message.Should().Be("Can only set sublocation recipients on the most recent order.");
        }

        public static IEnumerable<object[]> SetSublocationRecipientsSingleOrderNotValidData()
        {
            Order completeOrder = CommonOrderFactory(45, 32);
            completeOrder.Completed = new DateTime(2024, 01, 03);

            Order terminatedOrder = CommonOrderFactory(11, 76);
            terminatedOrder.IsTerminated = true;

            Order deletedOrder = CommonOrderFactory(51, 55);
            deletedOrder.IsDeleted = true;

            Order expiredOrder = CommonOrderFactory(36, 66);
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
                    CommonOrganisationFactory(61), CommonOrderFactory(61, 78),
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
                    CommonOrganisationFactory(45), CommonOrderFactory(45, 32),
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
                    CommonOrganisationFactory(75), CommonOrderFactory(75, 14),
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
                    CommonOrganisationFactory(81), CommonOrderFactory(81, 9),
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
                Id = customId == 0 ? CommonOrderNumber : customId,
                InternalIdentifier = CommonOrganisationInternalIdentifier,
                ExternalIdentifier = CommonOrganisationExternalIdentifier,
                Name = "A Local ICB",
            };
        }

        private static Order CommonOrderFactory(
            int customOrganisationId = 0,
            int customId = 0,
            int customOrderNumber = 0,
            int customRevision = 0,
            ICollection<OrderSublocation> orderSublocations = null)
        {
            var random = new Random();
            return new Order
            {
                Id = customId == 0 ? random.Next() : customId,
                OrderNumber = customOrderNumber == 0 ? CommonOrderNumber : customId,
                Revision = customOrderNumber == 0 ? customRevision : 1,
                Description = $"An order {customId}",
                OrderingPartyId = customOrganisationId == 0 ? CommonOrganisationId : customOrganisationId,
                SelectedFramework = new EntityFramework.Catalogue.Models.Framework { Id = random.Next().ToString() },
                OrderSublocations = orderSublocations,
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
                OrderId = customOrderId == 0 ? CommonOrderNumber : customOrderId,
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
