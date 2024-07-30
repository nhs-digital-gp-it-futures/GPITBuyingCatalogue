using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates
{
    public static class EditDatesModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            var organisations = order.OrderRecipients
                .Select(item => new ServiceRecipient() { OrgId = item.OdsCode, Location = Guid.NewGuid().ToString() })
                .ToList();
            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var model = new EditDatesModel(new OrderWrapper(order), catalogueItemId, organisations);

            model.InternalOrgId.Should().Be(order.OrderingParty.InternalIdentifier);
            model.CallOffId.Should().Be(order.CallOffId);
            model.OrderType.Should().Be(order.OrderType);
            model.SolutionName.Should().Be(order.OrderType.GetSolutionNameFromOrder(order));
            model.CatalogueItemId.Should().Be(catalogueItemId);
            model.DeliveryDate.Should().Be(order.DeliveryDate);
        }

        [Theory]
        [MockAutoData]
        public static void MergerType_RecipientsCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceMerger;
            var organisations = order.OrderRecipients
                .Select(item => new ServiceRecipient() { OrgId = item.OdsCode, Location = Guid.NewGuid().ToString() })
                .ToList();
            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var model = new EditDatesModel(new OrderWrapper(order), catalogueItemId, organisations);

            model.Recipients.Count.Should().Be(1);
            model.Recipients.First().Key.Should().Be("Service Recipients to be merged");
            model.Recipients.First().Value.Length.Should().Be(order.OrderRecipients.Count);
        }

        [Theory]
        [MockAutoData]
        public static void SplitType_RecipientsCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceSplit;
            var organisations = order.OrderRecipients
                .Select(item => new ServiceRecipient() { OrgId = item.OdsCode, Location = Guid.NewGuid().ToString() })
                .ToList();
            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var model = new EditDatesModel(new OrderWrapper(order), catalogueItemId, organisations);

            model.Recipients.Count.Should().Be(1);
            model.Recipients.First().Key.Should().Be("Service Recipients receiving patients");
            model.Recipients.First().Value.Length.Should().Be(order.OrderRecipients.Count);
        }

        [Theory]
        [MockAutoData]
        public static void Solution_RecipientsCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.Solution;
            var organisations = order.OrderRecipients
                .Select(item => new ServiceRecipient() { OrgId = item.OdsCode, Location = Guid.NewGuid().ToString() })
                .ToList();
            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var model = new EditDatesModel(new OrderWrapper(order), catalogueItemId, organisations);
            model.Recipients.Count.Should().Be(organisations.Count);
            model.Recipients.Select(x => x.Key).Should().BeEquivalentTo(organisations.Select(x => x.Location));
            model.Recipients.SelectMany(x => x.Value).Count().Should().Be(order.OrderRecipients.Count);
        }
    }
}
