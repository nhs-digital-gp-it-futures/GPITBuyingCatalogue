using System.Linq;
using FluentAssertions;
using LinqKit;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
            var orderItem = order.OrderItems.First();
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var model = new EditDatesModel(new OrderWrapper(order), orderItem.Id);

            model.InternalOrgId.Should().Be(order.OrderingParty.InternalIdentifier);
            model.CallOffId.Should().Be(order.CallOffId);
            model.OrderType.Should().Be(order.OrderType);
            model.SolutionName.Should().Be(order.OrderType.GetSolutionNameFromOrder(order));
            model.OrderItemId.Should().Be(orderItem.Id);
            model.DeliveryDate.Should().Be(order.DeliveryDate);
            model.CatalogueItemType.Should().Be(orderItem.CatalogueItem.CatalogueItemType);
            model.Description.Should().Be(orderItem.CatalogueItem.Name);
            model.CatalogueItemTypeSuffix.Should().Be("Catalogue solution");
        }

        [Theory]
        [MockAutoData]
        public static void MergerType_RecipientsCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceMerger;

            var orderItemId = order.OrderItems.First().Id;

            var model = new EditDatesModel(new OrderWrapper(order), orderItemId);

            model.Recipients.Count.Should().Be(1);
            model.Recipients.First().Key.Should().Be("Service recipients to be merged");
            model.Recipients.First().Value.Length.Should().Be(order.FlattenedRecipients.Count());
        }

        [Theory]
        [MockAutoData]
        public static void SplitType_RecipientsCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceSplit;

            var orderItemId = order.OrderItems.First().Id;

            var model = new EditDatesModel(new OrderWrapper(order), orderItemId);

            model.Recipients.Count.Should().Be(1);
            model.Recipients.First().Key.Should().Be("Service recipients receiving patients");
            model.Recipients.First().Value.Length.Should().Be(order.FlattenedRecipients.Count());
        }

        [Theory]
        [MockAutoData]
        public static void Solution_RecipientsCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.Solution;

            var orderItemId = order.OrderItems.First().Id;

            var expectedTotalRecipientCount = order.FlattenedRecipients.Count();

            var model = new EditDatesModel(new OrderWrapper(order), orderItemId);
            model.Recipients.Count.Should().Be(order.OrderItems.Count);
            model.Recipients.Select(x => x.Key)
                .Should()
                .BeEquivalentTo(
                    order.OrderSublocations.Select(x => x.SublocationOrganisation.Name),
                    opt => opt.WithoutStrictOrdering());
            model.Recipients.SelectMany(x => x.Value).Count().Should().Be(expectedTotalRecipientCount);
        }

        [Theory]
        [MockAutoData]
        public static void NullDates_RecipientsCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceSplit;
            order.FlattenedRecipients.ForEach(x =>
                x.OrderItemSublocationRecipients.ForEach(y => y.DeliveryDate = null));

            var orderItemId = order.OrderItems.First().Id;

            var model = new EditDatesModel(new OrderWrapper(order), orderItemId);

            model.Recipients.Count.Should().Be(1);
            model.Recipients.First().Value.ForEach(x => x.Day.Should().Be($"{order.DeliveryDate.Value.Day:00}"));
            model.Recipients.First().Value.ForEach(x => x.Month.Should().Be($"{order.DeliveryDate.Value.Month:00}"));
            model.Recipients.First().Value.ForEach(x => x.Year.Should().Be($"{order.DeliveryDate.Value.Year:0000}"));
        }
    }
}
