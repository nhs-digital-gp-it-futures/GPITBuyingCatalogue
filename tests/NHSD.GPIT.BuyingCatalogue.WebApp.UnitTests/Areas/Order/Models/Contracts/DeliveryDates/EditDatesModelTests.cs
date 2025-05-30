using System.Linq;
using FluentAssertions;
using LinqKit;
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
            EntityFramework.Ordering.Models.Order order
        )
        {
            CatalogueItemId catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var model = new EditDatesModel(new OrderWrapper(order), catalogueItemId);

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

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var model = new EditDatesModel(new OrderWrapper(order), catalogueItemId);

            model.Recipients.Count.Should().Be(1);
            model.Recipients.First().Key.Should().Be("Service Recipients to be merged");
            model.Recipients.First().Value.Length.Should().Be(order.FlattenedRecipients.Count());
        }

        [Theory]
        [MockAutoData]
        public static void SplitType_RecipientsCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceSplit;

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var model = new EditDatesModel(new OrderWrapper(order), catalogueItemId);

            model.Recipients.Count.Should().Be(1);
            model.Recipients.First().Key.Should().Be("Service Recipients receiving patients");
            model.Recipients.First().Value.Length.Should().Be(order.FlattenedRecipients.Count());
        }

        [Theory]
        [MockAutoData]
        public static void Solution_RecipientsCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.Solution;

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var expectedTotalRecipientCount = order.FlattenedRecipients.Count() * order.OrderItems.Count();

            var model = new EditDatesModel(new OrderWrapper(order), catalogueItemId);
            model.Recipients.Count.Should().Be(order.OrderItems.Count);
            model.Recipients.Select(x => x.Key)
                .Should()
                .BeEquivalentTo(order.OrderItems.Select(x => x.CatalogueItem.Name), opt => opt.WithoutStrictOrdering());
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

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            var model = new EditDatesModel(new OrderWrapper(order), catalogueItemId);

            model.Recipients.Count.Should().Be(1);
            model.Recipients.First().Value.ForEach(x => x.Day.Should().Be($"{order.DeliveryDate.Value.Day:00}"));
            model.Recipients.First().Value.ForEach(x => x.Month.Should().Be($"{order.DeliveryDate.Value.Month:00}"));
            model.Recipients.First().Value.ForEach(x => x.Year.Should().Be($"{order.DeliveryDate.Value.Year:0000}"));
        }
    }
}
