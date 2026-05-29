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
                    CommonOrganisationFactory(654),
                    CommonOrderFactory(22, 654, 0, 0, [CommonOrderSublocationFactory(22, "XXXX", [])]), 0,
                ],

                [
                    CommonOrganisationFactory(621), CommonOrderFactory(
                        65,
                        621,
                        0,
                        0,
                        [
                            CommonOrderSublocationFactory(
                                65,
                                "XXXY",
                                [
                                    CommonOrderSublocationRecipientFactory(65, "BAAA", "XXXY"),
                                    CommonOrderSublocationRecipientFactory(65, "BAAB", "XXXY"),
                                ]),
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
            int expectedCount,
            [Frozen] BuyingCatalogueDbContext context,
            OrderSublocationService service)
        {
            order.OrderingParty = organisation;

            context.Add(order);

            OrderSublocation workingSublocation = order.OrderSublocations.First();

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var count = await service.GetCountForOrderSublocationRecipients(
                organisation.ExternalIdentifier,
                order.Id,
                workingSublocation.SublocationOdsCode);

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
                    CommonOrganisationFactory(78),
                    CommonOrderFactory(45, 78, 0, 0, [CommonOrderSublocationFactory(45, "XXXA", [], true)]),

                    new HashSet<string> { "AAAA" },
                    CommonServiceRecipientFactory("AAAA", "XXXA"),
                ],
            ];
        }

        public static IEnumerable<object[]> SetSublocationRecipientsData()
        {
            return
            [

                // Adds
                [
                    CommonOrganisationFactory(32), CommonOrderFactory(45, 32),
                    CommonOrderSublocationFactory(45, "XXXX", [], true),
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
                        45,
                        "XXXX",
                        [
                            CommonOrderSublocationRecipientFactory(45, "AAAA", "XXXX", true),
                            CommonOrderSublocationRecipientFactory(45, "AAAB", "XXXX", true),
                        ],
                        true),
                ],

                // Removes
                [
                    CommonOrganisationFactory(14), CommonOrderFactory(75, 14),
                    CommonOrderSublocationFactory(
                        75,
                        "YXXX",
                        [
                            CommonOrderSublocationRecipientFactory(75, "BAAA", "YXXX"),
                            CommonOrderSublocationRecipientFactory(75, "BAAB", "YXXX"),
                            CommonOrderSublocationRecipientFactory(75, "BAAC", "YXXX"),
                        ],
                        true),

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
                        75,
                        "YXXX",
                        [
                            CommonOrderSublocationRecipientFactory(75, "BAAA", "YXXX", true),
                            CommonOrderSublocationRecipientFactory(75, "BAAB", "YXXX", true),
                        ],
                        true),
                ],

                // Adds and removes
                [
                    CommonOrganisationFactory(9), CommonOrderFactory(81, 9),
                    CommonOrderSublocationFactory(
                        81,
                        "ZXXX",
                        [
                            CommonOrderSublocationRecipientFactory(81, "CAAA", "ZXXX"),
                            CommonOrderSublocationRecipientFactory(81, "CAAB", "ZXXX"),
                            CommonOrderSublocationRecipientFactory(81, "CAAC", "ZXXX"),
                        ],
                        true),

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
                        81,
                        "ZXXX",
                        [
                            CommonOrderSublocationRecipientFactory(81, "CAAA", "ZXXX", true),
                            CommonOrderSublocationRecipientFactory(81, "CAAB", "ZXXX", true),
                            CommonOrderSublocationRecipientFactory(81, "CAAD", "ZXXX", true),
                            CommonOrderSublocationRecipientFactory(81, "CAAF", "ZXXX", true),
                        ],
                        true),
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
            [Frozen] IOrderService orderService,
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

            orderService.GetOrderWithCatalogueItemAndPrices(order.CallOffId, order.OrderingParty.InternalIdentifier)
                .Returns(new OrderWrapper(order));

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
            int customId = 0,
            int customOrganisationId = 0,
            int customOrderNumber = 0,
            int customRevision = 0,
            ICollection<OrderSublocation> orderSublocations = null,
            bool isComplete = false)
        {
            var random = new Random();
            return new Order
            {
                Id = customId == 0 ? random.Next() : customId,
                OrderNumber = customOrderNumber == 0 ? CommonOrderNumber : customId,
                Revision = customRevision,
                Description = $"An order {customId}",
                OrderingPartyId = customOrganisationId == 0 ? CommonOrganisationId : customOrganisationId,
                SelectedFramework = new EntityFramework.Catalogue.Models.Framework { Id = random.Next().ToString() },
                OrderSublocations = orderSublocations,
                Completed = isComplete ? new DateTime(2023, 01, 01) : null,
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

        private static ServiceRecipient CommonServiceRecipientFactory(string orgId, string locationOrgId)
        {
            return new ServiceRecipient { OrgId = orgId, LocationOrgId = locationOrgId };
        }
    }
}
