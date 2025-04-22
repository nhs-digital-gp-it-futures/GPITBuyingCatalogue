using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
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
            sublocation.OrderId = order.Id;
            sublocation.OwnerOdsCode = organisation.ExternalIdentifier;

            sublocation.SublocationOrganisation = sublocationOrganisation;
            sublocation.SublocationRecipients = orderSublocationRecipients;

            orderSublocationRecipients.ForEach(x =>
                x.RecipientOdsOrganisation = CommonEntityOdsOrganisationFactory(x.RecipientOdsCode));

            context.Add(sublocation);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            OrderSublocation actualSublocation = await service.GetOrderSublocationWithRecipients(
                organisation.ExternalIdentifier,
                order.CallOffId,
                sublocation.SublocationOdsCode);

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
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(OrderSublocationsWithRecipientsForCount))]
        public static async Task GetCountForOrderSublocationRecipients_ReturnsCount(
            Organisation organisation,
            Order Order,
            OrderSublocation sublocation,
            int expectedCount,
            [Frozen] BuyingCatalogueDbContext context,
            OrderSublocationService service)
        {
            Order.OrderingParty = organisation;

            sublocation.Order = Order;
            sublocation.OrderId = Order.Id;
            sublocation.OwnerOdsCode = organisation.ExternalIdentifier;

            context.Add(sublocation);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var count = await service.GetCountForOrderSublocationRecipients(
                organisation.ExternalIdentifier,
                Order.CallOffId,
                sublocation.SublocationOdsCode);

            Assert.Equal(expectedCount, count);
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
                OrderingPartyId = customOrganisationId == 0 ? CommonOrganisationId : customOrganisationId,
                Description = "Order for Ordery things",
            };
        }

        private static OrderSublocationRecipient CommonOrderSublocationRecipientFactory(
            string recipientOdsCode,
            string parentSublocationOdsCode,
            int OrderId = 0,
            bool hasOrganisation = false)
        {
            return new OrderSublocationRecipient
            {
                Order = new Order(),
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
